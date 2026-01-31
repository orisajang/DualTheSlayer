using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition :MonoBehaviour
{
    //플레이어 매니저와 현재 객체의 PhotonView
    private PlayerManager _playerManager;
    private PhotonView _pv;
    Dictionary<eConditionType, PlayerConditionUI> _conditionTypeDic;

    //기타 상태이상들
    //도트힐 스택 (턴이 시작될때마다 힐을 함)
    //public int dotHealAmount { get; private set; } //힐량
    //public int dotHealDuration { get; private set; } //힐 지속턴

    //출혈 스택 (카드를 낼때마다 데미지를 받음)
    private const int INVALID_BLEED_APPLIER_ID = -1; //출혈을 보유한 사람이 없는지 체크하기위해
    //public int bleedingAmount { get; private set; } //출혈 횟수
    //public int bleedingDuration { get; private set; } //출혈 지속턴
    public int BleedApplierId { get; private set; } = INVALID_BLEED_APPLIER_ID;  //출혈을 부여한사람 ID (데미지처리를 위해서)

    public void Init(PlayerManager playerManager, PhotonView photonView, Dictionary<eConditionType, PlayerConditionUI> playerConditionTypeDic)
    {
        _playerManager = playerManager;
        _pv = photonView;
        _conditionTypeDic = playerConditionTypeDic;
    }
    
    //1. 상태이상 체크및 RPC
    public void CheckPlayerHealing()
    {
        //한번도 부여된적이 없다면 return
        if (_conditionTypeDic == null || !_conditionTypeDic.ContainsKey(eConditionType.DotHealing)) return;
        if (_conditionTypeDic[eConditionType.DotHealing].Duration > 0)
        {
            //힐 1회 발동시키고, 횟수 감소
            _playerManager.HealingPlayerSelf(_conditionTypeDic[eConditionType.DotHealing].Amount); //이벤트하나 추가해서 Invoke로 할면좋을지? 고민중
            DecreseHealDuration();
        }
    }

    public void CheckBleedingStatus()
    {
        //예외처리: 아직 출혈이 한번도 부여된적이 없었다면 return
        if (!_conditionTypeDic.ContainsKey(eConditionType.Bleeding)) return;
        //내 네트워크 객체이고, 출혈횟수가 0보다 클때만 발동
        if(_pv.IsMine && _conditionTypeDic[eConditionType.Bleeding].Duration > 0)
        {
            _pv.RPC(nameof(CheckBleedingStatusRPC), RpcTarget.All);
        }
    }
    [PunRPC]
    public void CheckBleedingStatusRPC()
    {
        Debug.Log($"실행자 ID: {_pv.Owner.ActorNumber} 출혈량 {_conditionTypeDic[eConditionType.Bleeding].Amount}, 횟수 {_conditionTypeDic[eConditionType.Bleeding].Duration}");
        if (_conditionTypeDic[eConditionType.Bleeding].Duration > 0)
        {
            //예외처리: 만약 출혈 부여자의 ID가 없는경우라면 이상함. 에러발생
            if (BleedApplierId == INVALID_BLEED_APPLIER_ID)
            {
                Debug.LogError("출혈 보유자의 ID가 없습니다. 에러발생");
                return;
            }
            //bleedingDuration -= 1;
            Debug.Log($"출혈 발동!! 데미지: {_conditionTypeDic[eConditionType.Bleeding].Amount} 남은출혈횟수: {_conditionTypeDic[eConditionType.Bleeding].Duration - 1}");
            //자기자신만 TakeDamage를 보내줌(1번만 보내야하므로)

            if (_pv.IsMine)
            {
                //bool isBuffEnd = _playerBuffTypeDic[eBuffType.Bleeding].ActivateBuffOnce();
                _pv.RPC(nameof(ActivateConditionTickOnceRPC), RpcTarget.All, eConditionType.Bleeding);
                _playerManager.TakeDamage(_conditionTypeDic[eConditionType.Bleeding].Amount, BleedApplierId);
            }
        }

        //출혈 해제되었을경우 ID값 다시 초기화
        if (_conditionTypeDic[eConditionType.Bleeding].Duration <= 0)
        {
            BleedApplierId = INVALID_BLEED_APPLIER_ID;
        }
    }

    //2. 상태이상 부여
    //출혈 상태 부여 (효과는 카드 낼때마다 발동됨 -> CheckUseCardAfterEffect에서 실제 발동)
    public void AddBleedingStatus(int amount, int duration, int cardCost, int applierActorNumber)
    {
        //여기서는 RPC로 출혈받는사람이 자기자신의 출혈수치만 증가시킴
        Debug.Log($"시작전 부여자: {applierActorNumber}, 피격자: {_pv.Owner.ActorNumber} 출혈량 {amount}, 횟수 {duration}");
        //이 메서드는 다른사람이 생성한 네트워크 객체에 접근해서 RPC를 쏴주는거라 IsMine이 무조건 false임. IsMine체크하지말것
        _pv.RPC(nameof(AddBleedingStatusRPC), RpcTarget.All, amount, duration, cardCost, applierActorNumber);
        Debug.Log($"출혈 photonView.IsMine 들어왔다");
    }
    [PunRPC]
    private void AddBleedingStatusRPC(int amount, int duration, int cardCost, int applierActorNumber)
    {
        Debug.Log($"RPC시작전 부여자: {applierActorNumber}, 피격자: {_pv.Owner.ActorNumber} 출혈량 {amount}, 횟수 {duration}");
        //bleedingAmount = amount;
        //bleedingDuration = duration;
        BleedApplierId = applierActorNumber;

        //UI생성
        _playerManager.CreateOrAddBuffStatus(eConditionType.Bleeding, amount, duration);
    }
    public void AddDotHealStatus(int amount, int duration, int cardCost)
    {
        if (_pv.IsMine)
        {
            _pv.RPC(nameof(AddDotHealStatusRPC), RpcTarget.All, amount, duration);
        }
    }
    [PunRPC]
    private void AddDotHealStatusRPC(int amount, int duration)
    {
        //dotHealAmount = amount;
        //dotHealDuration = duration;

        //UI생성
        _playerManager.CreateOrAddBuffStatus(eConditionType.DotHealing, amount, duration);
    }
    //3. 스택 1회 소모
    [PunRPC]
    public void ActivateConditionTickOnceRPC(eConditionType type)
    {
        //RPC로 상태이상 횟수를 1회 적용했다는거를 알린다
        bool isBuffEnd = false;
        isBuffEnd = _conditionTypeDic[type].ActivateConditionOnce();
        if (isBuffEnd) _conditionTypeDic.Remove(type);
    }
    //회복 버프효과 1회 갱신
    public void DecreseHealDuration()
    {
        //dotHealDuration -= 1;
        //지속시간 다 끝나면 초기화
        //if (dotHealDuration <= 0)
        //{
        //    dotHealAmount = 0;
        //}
        //UI 1회 발동
        //_playerConditionTypeDic[eConditionType.DotHealing].ActivateConditionOnce();
        if (_pv.IsMine)
        {
            _pv.RPC(nameof(ActivateConditionTickOnceRPC), RpcTarget.All, eConditionType.DotHealing);
        }
    }
}

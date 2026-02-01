using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition :MonoBehaviour
{
    //플레이어 매니저와 현재 객체의 PhotonView
    private PlayerManager _playerManager;
    private PhotonView _pv;
    //플레이어 상태이상UI 딕셔너리 (이곳에 접근해서 타입별 상태이상량, 횟수에 접근할 수 있음)
    Dictionary<eConditionType, PlayerConditionUI> _conditionTypeDic;
    //플레이어 상태이상중에서 다음턴에 무조건 삭제되는것들(힘, 방어도, 등등.. 이런 상태이상버프들은 지속시간 스택불가. 다음턴에 사라져야함)
    Dictionary<eConditionType, bool> _conditionDurationStackableDic = new Dictionary<eConditionType, bool>()
    {
        { eConditionType.None, false},
        { eConditionType.DotHealing, true},
        { eConditionType.Bleeding, true},
        { eConditionType.Power, false},
    };

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
    private void CheckBleedingStatusRPC()
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
                _playerManager.TakeDamage(_conditionTypeDic[eConditionType.Bleeding].Amount, BleedApplierId);
                //출혈 해제되었을경우 ID값 다시 초기화
                if (_conditionTypeDic[eConditionType.Bleeding].Duration - 1 <= 0)
                {
                    BleedApplierId = INVALID_BLEED_APPLIER_ID;
                }

                _pv.RPC(nameof(ActivateConditionTickOnceRPC), RpcTarget.All, eConditionType.Bleeding);
               
            }
        }
    }
    //상태이상 중에서 무조건 다음턴에 삭제되어야하는 것들(힘,방어도 등등.. 이런것들을 한번에 체크해서 없애주기위한 메서드)
    public void CheckDisStackableCondtions()
    {
        //예외처리(상태이상 체크이므로 조건에 맞아야 들어올수있음. 초기에 들어오지않게 return)
        if (_pv == null || !_pv.IsMine || _conditionTypeDic == null) return;

        foreach (eConditionType type in _conditionDurationStackableDic.Keys)
        {
            bool isStackAble = _conditionDurationStackableDic[type];
            if (isStackAble == false)
            {
                if (_conditionTypeDic.ContainsKey(type))
                {
                    //다음턴에 삭제되는 상태이상들은 무조건 지속시간 1이므로 1회 감소시키면 사라지도록 구조가 짜져있음
                    _pv.RPC(nameof(ActivateConditionTickOnceRPC), RpcTarget.All, type); //무조건 버프가 사라질거임
                }
            }
        }
    }
    //플레이어의 힘 수치를 가져옴
    public int GetPlayerPower()
    {
        //예외처리
        if(_conditionTypeDic == null || !_conditionTypeDic.ContainsKey(eConditionType.Power))
        {
            return 0;
        }
        if(_conditionTypeDic[eConditionType.Power].Duration ==0)
        {
            Debug.LogError("Power횟수가 0임. 여기 오면 안되는데 뭔가 에러");
            return 0;
        }
        //정상적이라면 바로 플레이어의 현재 힘을 보내준다
        return _conditionTypeDic[eConditionType.Power].Amount;
    }

    //2. 상태이상 부여
    public void AddConditionStatus(eConditionType conditionType, int amount, int duration, int applierActorNumber)
    {
        //RPC로 보내주기위한 일반 메서드 사용
        Debug.Log($"상태이상추가! 타입:{conditionType} 시작전 부여자: {applierActorNumber}, 피격자: {_pv.Owner.ActorNumber} 출혈량 {amount}, 횟수 {duration}");
        //지속시간 쌓일수있는 버프 유형인지 체크
        bool isStackAble = _conditionDurationStackableDic[conditionType];

        if (conditionType == eConditionType.DotHealing)
        {
            if (_pv.IsMine)
            {
                _pv.RPC(nameof(AddConditionRPC), RpcTarget.All, conditionType, amount, duration, applierActorNumber, isStackAble);
            }
        }
        else
        {
            _pv.RPC(nameof(AddConditionRPC), RpcTarget.All, conditionType, amount, duration, applierActorNumber, isStackAble);
        }
    }
    [PunRPC]
    private void AddConditionRPC(eConditionType conditionType, int amount, int duration, int applierActorNumber, bool isStackAble)
    {
        if (conditionType == eConditionType.Bleeding) BleedApplierId = applierActorNumber;

        _playerManager.CreateOrAddBuffStatus(conditionType, amount, duration, isStackAble);
        //상태이상인 경우 카드 텍스트들을 다 확인해준다
        _playerManager.UpdateAllCardDescription(); //버프설정될때 자신의 카드의 Text도 변경해줌
    }

    //3. 스택 1회 소모
    [PunRPC]
    private void ActivateConditionTickOnceRPC(eConditionType type)
    {
        //RPC로 상태이상 횟수를 1회 적용했다는거를 알린다
        bool isBuffEnd = false;
        isBuffEnd = _conditionTypeDic[type].ActivateConditionOnce();
        if (isBuffEnd)
        {
            _conditionTypeDic.Remove(type);
            _playerManager.UpdateAllCardDescription(); //버프효과 종료되었으므로 카드의 Text도 변경해줌
        }
    }
    //회복 버프효과 1회 갱신
    private void DecreseHealDuration()
    {
        if (_pv.IsMine)
        {
            _pv.RPC(nameof(ActivateConditionTickOnceRPC), RpcTarget.All, eConditionType.DotHealing);
        }
    }
}

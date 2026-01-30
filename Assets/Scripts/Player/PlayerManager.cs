using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    //플레이어의 HP.. 등등이 있어야한다 일단은 보류
    //플레이어 ID (포톤에서 불러와서 설정할 예정, 일단은 임시 아이디 넣자)
    public int PlayerID { get; private set; }
    //플레이어의 덱
    PlayerDeck playerDeck;
    //플레이어 손패
    List<CardInstance> playerHand = new List<CardInstance>();

    //시작 플레이 카드
    [SerializeField,Tooltip("플레이어 시작 카드 수")] int startCardCount = 5;
    [SerializeField] RectTransform _cardSpawnPosition;
    [SerializeField] GameObject _cardPrefab;

    //카드 내는 제한선
    [SerializeField] RectTransform _useCardLine;

    //게임화면에서 에너지를 표시하는 텍스트UI가 어디에있는지
    [SerializeField] TextMeshProUGUI _energyTextUI;
    //최대 카드 보유가능 갯수 
    [SerializeField] int limitMaxCard = 10;


    //플레이어의 스탯
    public int level { get; private set; }
    public int exp { get; private set; }
    public int maxHp { get; private set; }
    public int currentHp { get; private set; }
    public int shield { get; private set; }
    public int currentEnergy { get; private set; }
    private int maxEnergy = 3; // 일단 무조건 3이라고 가정하고 사용
    //기타 상태이상들
    //도트힐 스택 (턴이 시작될때마다 힐을 함)
    public int dotHealAmount { get; private set; } //힐량
    public int dotHealDuration { get; private set; } //힐 지속턴

    //출혈 스택 (카드를 낼때마다 데미지를 받음)
    private const int INVALID_BLEED_APPLIER_ID = -1; //출혈을 보유한 사람이 없는지 체크하기위해
    public int bleedingAmount { get; private set; } //출혈 횟수
    public int bleedingDuration { get; private set; } //출혈 지속턴
    public int BleedApplierId { get; private set; } = INVALID_BLEED_APPLIER_ID;  //출혈을 부여한사람 ID (데미지처리를 위해서)


    //플레이어의 HP바
    [SerializeField] PlayerHpBar _playerHpBar;

    //플레이어가 죽었을때 처리하기 위해서 이벤트 추가
    public event Action<int, int> OnPlayerDead;

    //플레이어의 버프UI생성위치
    //[SerializeField] PlayerBuffUI _playerBuffUI; //이거 못함. 버프가 적용될때 이 스크립트를 생성해야하는것이기 떄문에
    //플레이어버프UI를 오브젝트풀로 생성해주는 스크립트
    [SerializeField] PlayerConditionSpawner _playerConditionSpawner;  
    //현재 해당 상태이상은 이 UI를 사용하고있다는것을 기억하기위해서 딕셔너리 사용, 버프 생성 및 저장해놓은 딕셔너리
    Dictionary<eConditionType, PlayerConditionUI> _playerConditionTypeDic = new Dictionary<eConditionType, PlayerConditionUI>();
    //상태이상 전략패턴으로 딕셔너리 캐싱
    Dictionary<eConditionType, ConditionStrategy> _conditionStrategyDic = new Dictionary<eConditionType, ConditionStrategy>();


    public int CalcMaxHP(int level)
    {
        return level * 10;
    }

    

    //public void Init(RectTransform cardSpawnPosition, GameObject cardPrefab, RectTransform useCardLine, string id, List<CardSOClass> deck)
    public void Init(PlayerConfig config)
    {
        PlayerID = config.id;
        //초기 설정
        if (!photonView.IsMine) return;
        _cardSpawnPosition = config.cardSpawnPosition;
        _cardPrefab = config.cardPrefab;
        _useCardLine = config.useCardLine;
        //덱 설정
        playerDeck = GetComponent<PlayerDeck>();
        playerDeck.SetCard(config.originDeck);
        //플레이어 초기 손패 5개 넣음
        for (int index = 0; index < startCardCount; index++)
        {
            DrawPlayerCard();
        }
        Debug.Log(playerHand.Count);
        //플레이어 정보 설정
        //InitPlayerStat(config.levelData); //RPC로 다같이하면될듯.
    }
    public void InitPlayerStat(PlayerLevelData levelData)
    {
        level = levelData.Level;
        exp = levelData.Exp;
        maxHp = CalcMaxHP(levelData.Level);
        currentHp = maxHp;
        shield = 0;

        UpdateHpBar();
    }
    [PunRPC]
    public void RPC_Init(int lvl, int expoint, PhotonMessageInfo info)
    {
        level = lvl;
        exp = expoint;
        maxHp = CalcMaxHP(lvl);
        currentHp = maxHp;
        shield = 0;

        UpdateHpBar();
    }
    //게임매니저에 자기자신을 설정해서 접근할수있도록
    public override void OnEnable()
    {
        base.OnEnable();
        //등록은 해주고 아래코드는 실행안함
        GameManager.Instance.SetPlayerManager(this, photonView.Owner.ActorNumber);
        if (!photonView.IsMine) return;
        
        //소유자의 번호를 넣어줘서 딕셔너리 등록할 수 있도록함
        
    }
    public override void OnDisable()
    {
        base.OnDisable();
        if (GameManager.isHaveInstance) GameManager.Instance.DeletePlayerManager(this, photonView.Owner.ActorNumber);
        if (!photonView.IsMine) return;
        
        
    } 
    //메서드 실행하면 카드를 1장 뽑는다
    public void DrawPlayerCard()
    {
        //카드를 뽑는거는 이 플레이어를 소유한 사람만 뽑아야하므로 예외처리
        if (!photonView.IsMine) return;
        //최대 카드 보유량을 넘어섰다면 return
        if (playerHand.Count >= limitMaxCard) return;

        //카드 데이터 가져오기 (현재 카드 코스트 감소할수있으므로 원본데이터와 따로 분리해서 인스턴스라는것을 만들어서 가져왔음)
        CardInstance instance = playerDeck.GetCard();
        //카드 인스턴스 생성 (현재 카드의 코스트가 감소할수있으므로 인스턴스를 만들어서 원본 데이터와 따로 관리
        //CardInstance instance = new CardInstance(cardInstanceData);
        //손패에 추가 (List)
        playerHand.Add(instance);
        //풀에서 카드를 1개 활성화해주고 카드 정보설정 
        CardView cardView = CardSpawner.Instance.GetCardByPool();
        cardView.Init(instance);
        //카드 Hover할때도 넣어주자
        CardHover cardHover = cardView.GetComponent<CardHover>();
        cardHover.Init(_useCardLine);
        //다음에 뭔가 스크립트 짤때마다 여기서 계속 초기화
        CardArrorwUI arrorUI = cardView.GetComponent<CardArrorwUI>();
        arrorUI.Init();
        CardInputController cardInputController = cardView.GetComponent<CardInputController>();
        cardInputController.Init();
    }
    //플레이어 턴이 시작될때 초기화해야할 속성을 모아둠
    public void SetPlyerTurnInit(TextMeshProUGUI text)
    {
        //에너지 UI 표시위치 설정 및 최대 에너지로 현재 에너지 설정
        _energyTextUI = text;
        currentEnergy = maxEnergy;
        //초기 자신의 행동력을 텍스트에 표시하는 RPC
        photonView.RPC(nameof(InitEnergyRPC), RpcTarget.All, currentEnergy);
        //카드를 한장 뽑는다
        DrawPlayerCard();

        //만약에 플레이어에게 도트힐 상태가 있었다면 힐을 해준다.
        CheckPlayerHealing();


    }
    //도트힐을 통해 턴시작 시 힐을 적용해야하는지 체크
    public void CheckPlayerHealing()
    {
        if(dotHealDuration > 0)
        {
            HealingPlayerSelf(dotHealAmount);
        }
    }
    //더이상 자신의 턴이 아닐때 초기화해야할 항목들 모아둠
    public void RemovePlayerTurnInit()
    {
        _energyTextUI = null;
    }
    public void UpdateHpBar()
    {
        //HP바 정보 업데이트
        float hpratio = 0;
        if (currentHp <= 0)
        {
            currentHp = 0;
            hpratio = 0;
        }
        else
        {
            hpratio = currentHp / (float)maxHp;
        }
        _playerHpBar.UpdateHPBarInfo(hpratio, currentHp, shield);
    }
    public void SetEnergyText(int energyText)
    {
        if (_energyTextUI == null)
        {
            Debug.Log("텍스트가 null");
            return;
        }
        
        _energyTextUI.text = energyText.ToString();
    }

    //사용된 카드를 플레이어 손패에서 지운다
    public void RemovePlayerHand(CardInstance usedCard)
    {
        //손패에서 제거
        playerHand.Remove(usedCard);
        Debug.Log($"플레이어의 손패갯수:{playerHand.Count}");
    }
    //카드를 낸 다음에 발동되어야 할 것들 모아둠.
    public void AfterUseCardBehavior(int cardCost)
    {
        //행동을 전부 실행한다음 행동력이 감소해야함
        DecreaseEnergy(cardCost);

        //카드 1회 내면 출혈효과 발동해야함. 추후  이펙트도 여기서 가능할듯 
        CheckBleedingStatus();
    }
    public void CheckBleedingStatus()
    {
        //내 네트워크 객체이고, 출혈횟수가 0보다 클때만 발동
        if(photonView.IsMine && bleedingDuration > 0)
        {
            photonView.RPC(nameof(CheckBleedingStatusRPC), RpcTarget.All);
        }
    }
    [PunRPC]
    public void CheckBleedingStatusRPC()
    {
        Debug.Log($"실행자 ID: {photonView.Owner.ActorNumber} 출혈량 {bleedingAmount}, 횟수 {bleedingDuration}");
        if (bleedingDuration > 0)
        {
            //예외처리: 만약 출혈 부여자의 ID가 없는경우라면 이상함. 에러발생
            if(BleedApplierId == INVALID_BLEED_APPLIER_ID)
            {
                Debug.LogError("출혈 보유자의 ID가 없습니다. 에러발생");
                return;
            }
            bleedingDuration -= 1;
            Debug.Log($"출혈 발동!! 데미지: {bleedingAmount} 남은출혈횟수: {bleedingDuration}");
            //자기자신만 TakeDamage를 보내줌(1번만 보내야하므로)
            
            if (photonView.IsMine)
            {
                //bool isBuffEnd = _playerBuffTypeDic[eBuffType.Bleeding].ActivateBuffOnce();
                photonView.RPC(nameof(ActivateConditionTickOnceRPC), RpcTarget.All, eConditionType.Bleeding);
                TakeDamage(bleedingAmount, BleedApplierId);
            }
        }
        
        //출혈 해제되었을경우 ID값 다시 초기화
        if(bleedingDuration <= 0)
        {
            bleedingAmount = 0;
            BleedApplierId = INVALID_BLEED_APPLIER_ID;
        }
    }
    [PunRPC]
    public void ActivateConditionTickOnceRPC(eConditionType type)
    {
        //RPC로 상태이상 횟수를 1회 적용했다는거를 알린다
        bool isBuffEnd = false;
        isBuffEnd = _playerConditionTypeDic[type].ActivateConditionOnce();
        if (isBuffEnd) _playerConditionTypeDic.Remove(type);
    }

    //쉴드 생성
    public void AddPlayerShield(int amount,int cardCost)
    {
        shield += amount;
        UpdateHpBar();
        //행동력 감소
        //DecreaseEnergy(cardCost);
    }
    //출혈 상태 부여 (효과는 카드 낼때마다 발동됨 -> CheckUseCardAfterEffect에서 실제 발동)
    public void AddBleedingStatus(int amount, int duration,int cardCost, int applierActorNumber)
    {
        //여기서는 RPC로 출혈받는사람이 자기자신의 출혈수치만 증가시킴
        Debug.Log($"시작전 부여자: {applierActorNumber}, 피격자: {photonView.Owner.ActorNumber} 출혈량 {amount}, 횟수 {duration}");
        //이 메서드는 다른사람이 생성한 네트워크 객체에 접근해서 RPC를 쏴주는거라 IsMine이 무조건 false임. IsMine체크하지말것
        photonView.RPC(nameof(AddBleedingStatusRPC), RpcTarget.All, amount, duration, cardCost, applierActorNumber);
        Debug.Log($"출혈 photonView.IsMine 들어왔다");
    }
    [PunRPC]
    private void AddBleedingStatusRPC(int amount, int duration, int cardCost, int applierActorNumber)
    {
        Debug.Log($"RPC시작전 부여자: {applierActorNumber}, 피격자: {photonView.Owner.ActorNumber} 출혈량 {amount}, 횟수 {duration}");
        bleedingAmount = amount;
        bleedingDuration = duration;
        BleedApplierId = applierActorNumber;

        //UI생성
        eConditionType bleedType = eConditionType.Bleeding;
        //이미 출혈이 존재한다면
        CreateOrAddBuffStatus(bleedType, amount, duration);
    }
    //버프가 없다면 만들어주고, 아니면 횟수를 추가해준다
    public void CreateOrAddBuffStatus(eConditionType buffType, int amount, int duration)
    {
        Debug.Log("전략패턴 시작");
        //전략패턴이 없을때만 새로 할당해주고 그 이후로는 계속 하나로 사용
        switch (buffType)
        {
            case eConditionType.Bleeding:
                if (!_conditionStrategyDic.ContainsKey(buffType)) _conditionStrategyDic.Add(buffType, new ConditionBleedingStrategy());
                break;
            case eConditionType.DotHealing:
                if (!_conditionStrategyDic.ContainsKey(buffType)) _conditionStrategyDic.Add(buffType, new ConditionHealStrategy());
                break;
        }
        _conditionStrategyDic[buffType].SetConditionUIData(_playerConditionSpawner, _playerConditionTypeDic, amount, duration, buffType);
    }
    public void AddDotHealStatus(int amount, int duration,int cardCost)
    {
        if(photonView.IsMine)
        {
            photonView.RPC(nameof(AddDotHealStatusRPC), RpcTarget.All, amount, duration);
        }
    }
    [PunRPC]
    private void AddDotHealStatusRPC(int amount, int duration)
    {
        dotHealAmount = amount;
        dotHealDuration = duration;

        //UI생성
        CreateOrAddBuffStatus(eConditionType.DotHealing, amount, duration);
    }
    //공격 받음
    public void TakeDamage(int amount,int attackerActorID)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, amount, attackerActorID);
        //TakeDamageRPC(amount);
    }
    //플레이어 회복
    public void HealingPlayerSelf(int amount)
    {
        //자기자신턴인 플레이어가 직접 RPC로 값들을 보내준다
        if(photonView.IsMine)
        {
            Debug.Log($"{photonView.Owner.ActorNumber}가 {amount}만큼 힐을 합니다");
            photonView.RPC(nameof(HealingPlayerSelfRPC), RpcTarget.All, amount);
        }
    }

    //행동력 감소 메서드 (RPC 실행용도)
    public void DecreaseEnergy(int cardCost)
    {
        if(photonView.IsMine)
        {
            photonView.RPC(nameof(DecreaseEnergyRPC), RpcTarget.All, cardCost);
        }
    }
    //자기자신 회복용도 RPC
    [PunRPC]
    public void HealingPlayerSelfRPC(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp;
        UpdateHpBar();

        dotHealDuration -= 1;
        //지속시간 다 끝나면 초기화
        if(dotHealDuration <= 0)
        {
            dotHealAmount = 0;
        }

        //UI 1회 발동
        //_playerConditionTypeDic[eConditionType.DotHealing].ActivateConditionOnce();
        if(photonView.IsMine)
        {
            photonView.RPC(nameof(ActivateConditionTickOnceRPC), RpcTarget.All, eConditionType.DotHealing);
        }
    }
    //행동력 감소할때 모두에게 알리기위해서 RPC
    [PunRPC]
    private void DecreaseEnergyRPC(int cardCost)
    {
        Debug.Log($"감소전 행동력 {currentEnergy} 비용 {cardCost}");
        currentEnergy -= cardCost;
        SetEnergyText(currentEnergy);
        Debug.Log($"{photonView.Owner.ActorNumber} 행동력: {currentEnergy}");
    }
    [PunRPC]
    private void InitEnergyRPC(int energy)
    {
        SetEnergyText(energy);
    }
    //데미지 받았을때 모두에게 데미지 받은사람 알리기위해서
    [PunRPC]
    public void TakeDamageRPC(int amount, int attackerActorID)
    {
        Debug.Log("TakeDamageRPC");
        CalcDamage(amount);
        UpdateHpBar();
        //HP 0이되었는지확인
        CheckHpZero(attackerActorID);
    }
    private void CalcDamage(int damage)
    {
        //쉴드량 계산해서 HP계산
        if( shield >damage)
        {
            shield -= damage;
        }
        else
        {
            damage -= shield;
            shield = 0;
            currentHp -= damage;
        }
    }
    //HP가 0이하가 되었다면 사망처리
    private void CheckHpZero(int attackerActorID)
    {
        if(currentHp <= 0)
        {
            if(photonView.IsMine)
            {
                OnPlayerDead?.Invoke(photonView.Owner.ActorNumber, attackerActorID);
            }
            //GameManager.Instance.inGameNetworkMgr.PlayerDead(photonView.Owner.ActorNumber);

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //서버에서 Write 상황일때
        {
            stream.SendNext(currentHp); //0번
            stream.SendNext(shield); //1번
        }
        else if (stream.IsReading) //서버에서 Read 상황일때
        {
            this.currentHp = (int)stream.ReceiveNext(); // 0번
            this.shield = (int)stream.ReceiveNext(); //1번 
            UpdateHpBar();
        }
    }
}

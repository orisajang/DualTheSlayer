using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
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
    RectTransform _cardSpawnPosition;
    GameObject _cardPrefab;

    //카드 내는 제한선
    RectTransform _useCardLine;

    //게임화면에서 에너지를 표시하는 텍스트UI가 어디에있는지
    TextMeshProUGUI _energyTextUI;
    //최대 카드 보유가능 갯수 
    [SerializeField] int limitMaxCard = 10;
    //플레이어가 데미지/힐 받을때 데미지량/힐량을 표시하는 텍스트가 생성되는 위치와 생성될 프리팹
    [SerializeField] GameObject _damageFontSpawnPosition;
    [SerializeField] GameObject _damageFontPrefab;


    //플레이어의 스탯
    public int level { get; private set; }
    public int exp { get; private set; }
    public int maxHp { get; private set; }
    public int currentHp { get; private set; }
    public int shield { get; private set; }
    public int currentEnergy { get; private set; }
    private int maxEnergy = 3; // 일단 무조건 3이라고 가정하고 사용
    //기타 상태이상들을 처리하는 클래스 
    [SerializeField] private PlayerCondition _playerCondition;
    public PlayerCondition PlayerCondition => _playerCondition;


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
    //각 상태이상별로 어떤 행동을 해야하는지 접근하기위해 전략패턴 + 딕셔너리 캐싱
    Dictionary<eConditionType, ConditionStrategy> _conditionStrategyDic = new Dictionary<eConditionType, ConditionStrategy>();

    //플레이어의 캐릭터 모델프리팹이 생성되는 위치
    [SerializeField] GameObject modelParent;

    //소리 설정 (추후에는 각자 SO가 SoundClip을 가지고있어서 거기서 PlayerManager의 메서드에 접근하는 식으로 하면 좋을듯)
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _attackClip;
    [SerializeField] AudioClip _healClip;
    [SerializeField] AudioClip _buffClip;
    [SerializeField] AudioClip _bleedClip;
    [SerializeField] AudioClip _shuffleClip;
    [SerializeField] AudioClip _shieldClip;
    public AudioSource AudioSource => _audioSource;
    public AudioClip AttackClip => _attackClip;
    public AudioClip BuffClip => _buffClip;
    public AudioClip BleedClip => _bleedClip;

    //특정 상태이상(버프)가 적용되면 모든카드를 조건에 따라 카드설명 변경해야함
    public void UpdateAllCardDescription()
    {
        //자기자신것이 아니라면 바로 return (자기자신의 손패의 카드의 정보를 설정하는 것이므로)
        if (!photonView.IsMine) return;

        //자신의 힘, 수비스킬 위력 증가 등 카드능력치에에 영향갈만한 변수들을 정의
        int plyPower = 0;
        
        //설정 (딕셔너리에서 설정값 꺼내서 넣어주기
        if(_playerConditionTypeDic.ContainsKey(eConditionType.Power))
        {
            plyPower = _playerConditionTypeDic[eConditionType.Power].Amount;
        }

        foreach(CardInstance card in playerHand)
        {
            StringBuilder sb = new StringBuilder();
            foreach(CardExecuteSO executeSO in card.ExecuteSOList)
            {
                Debug.Log("돌아가면서 힘을 체크합니다");
                //바뀐 카드 정보들을 sb에 담음
                sb.Append(executeSO.CardSetDescription(plyPower));
            }
            //데이터 수정 (수정하면 자동으로 UI도 이벤트로인해 바뀜)
            card.SetCardInstanceData(sb.ToString());
        }
    }

    private void Start()
    {
        _playerCondition.Init(this, photonView, _playerConditionTypeDic);
    }

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
    [PunRPC]
    public void RPC_InitCharacterModel(int index)
    {
        //회전 설정
        //0번째 위치일경우 왼쪽 -> 오른쪽을 바라보므로 90도, 1번째는 오른쪽 -> 왼쪽 바라보므로 180도 설정
        if (index == 0) { modelParent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0)); }
        else { modelParent.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0)); }
        GameObject characterModelPrefab = GameManager.Instance.CharacterModelPrefabs[index];
        Instantiate(characterModelPrefab, modelParent.transform);

        
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

        //카드 소리 재생
        SoundManager.Instance.PlayEffectSound(_audioSource, _shuffleClip);
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

        //상태이상 체크
        if(_playerCondition != null)
        {
            //만약에 플레이어에게 도트힐 상태가 있었다면 힐을 해준다.
            _playerCondition.CheckPlayerHealing();
            //플레이어 턴 시작하자마자 없어져야하는 상태이상들 체크
            _playerCondition.CheckDisStackableCondtions();
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
        _playerCondition.CheckBleedingStatus();
    }

    //버프가 없다면 만들어주고, 아니면 횟수를 추가해준다
    public void CreateOrAddBuffStatus(eConditionType buffType, int amount, int duration, bool isStackAble)
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
            case eConditionType.Power:
                if (!_conditionStrategyDic.ContainsKey(buffType)) _conditionStrategyDic.Add(buffType, new ConditionPowerStrategy());
                break;
        }
        _conditionStrategyDic[buffType].SetConditionUIData(_playerConditionSpawner, _playerConditionTypeDic, amount, duration, buffType, isStackAble);
    }
    

    //쉴드 생성
    public void AddPlayerShield(int amount,int cardCost)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(AddPlayerShieldRPC), RpcTarget.All, amount, cardCost);
        }
    }
    [PunRPC]
    private void AddPlayerShieldRPC(int amount, int cardCost)
    {
        shield += amount;
        UpdateHpBar();
        //소리재생
        SoundManager.Instance.PlayEffectSound(_audioSource, _shieldClip);
    }

    //데미지, 혹은 힐량이 몇만큼 적용되었는지 알리기위한 UI를 생성한다
    public void MakeDamageFontUIForShow(int amount, bool isDamage)
    {
        UIDamageString uIDamageString = Instantiate(_damageFontPrefab, _damageFontSpawnPosition.transform).GetComponent<UIDamageString>();
        uIDamageString.SetAndStartDamageText(amount, isDamage);
    }
    
    //공격 받음
    public void TakeDamage(int amount,int attackerActorID)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, amount, attackerActorID);
        //TakeDamageRPC(amount);
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
        //얼마만큼 데미지를 받았는지 UI를 띄운다
        MakeDamageFontUIForShow(amount,true);
    }
    //모두가 플레이어 공격 소리를 들어야 하므로 RPC메서드 추가
    [PunRPC]
    private void PlayPlayerAttackSoundRPC()
    {
        //소리 재생
        SoundManager.Instance.PlayEffectSound(_audioSource, _attackClip);
    }
    
    public void PlayPlayerAttackSound()
    {
        photonView.RPC(nameof(PlayPlayerAttackSoundRPC), RpcTarget.All);
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
    //자기자신 회복용도 RPC
    [PunRPC]
    public void HealingPlayerSelfRPC(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp;
        UpdateHpBar();
        //얼마만큼 힐을 받았는지 UI를 띄운다
        MakeDamageFontUIForShow(amount, false);
        //사운드 재생
        SoundManager.Instance.PlayEffectSound(_audioSource,_healClip);
    }
    //행동력 감소 메서드 (RPC 실행용도)
    public void DecreaseEnergy(int cardCost)
    {
        if(photonView.IsMine)
        {
            photonView.RPC(nameof(DecreaseEnergyRPC), RpcTarget.All, cardCost);
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
   
}

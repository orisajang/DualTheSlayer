using Photon.Pun;
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
    List<CardSOClass> playerHand = new List<CardSOClass>();

    //시작 플레이 카드
    [SerializeField,Tooltip("플레이어 시작 카드 수")] int startCardCount = 5;
    [SerializeField] RectTransform _cardSpawnPosition;
    [SerializeField] GameObject _cardPrefab;

    //카드 내는 제한선
    [SerializeField] RectTransform _useCardLine;

    //
    [SerializeField] TextMeshProUGUI _energyTextUI;

    //플레이어의 스탯
    public int level { get; private set; }
    public int exp { get; private set; }
    public int maxHp { get; private set; }
    public int currentHp { get; private set; }
    public int shield { get; private set; }
    public int currentEnergy { get; private set; }
    private int maxEnergy = 3; // 일단 무조건 3이라고 가정하고 사용
    //기타 상태이상들
    public int AttackBufValue;

    //플레이어의 HP바
    [SerializeField] PlayerHpBar _playerHpBar;

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
        playerDeck.SetCard(config.deck);
        //플레이어 초기 손패 5개 넣음
        for (int index = 0; index < startCardCount; index++)
        {
            SetPlayerHand();
        }
        Debug.Log(playerHand.Count);
        //플레이어 정보 설정
        //InitPlayerStat(config.levelData); //RPC로 다같이하면될듯.
    }
    public void InitPlayerStat(PlayerLevelData levelData)
    {
        level = levelData.level;
        exp = levelData.exp;
        maxHp = CalcMaxHP(levelData.level);
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
        GameManager.Instance.DeletePlayerManager(this, photonView.Owner.ActorNumber);
        if (!photonView.IsMine) return;
        
        
    } 
    public void SetPlayerHand()
    {
        //카드 데이터 가져오기
        CardSOClass cardSOData = playerDeck.GetCard();
        //카드 인스턴스 생성 (현재 카드의 코스트가 감소할수있으므로 인스턴스를 만들어서 원본 데이터와 따로 관리
        CardInstance instance = new CardInstance(cardSOData);
        //손패에 추가 (List)
        playerHand.Add(cardSOData);
        //테스트용. 소환해본다
        CardView cardView = CardSpawner.Instance.GetCardByPool();
        //이제 MVP에 있는곳 코드랑 연관해서 카드정보를 Set
        //초기화 (View를 초기화하면 안의 presenter과 Model도 생성및 초기화해줌)
        //추후에 초기화하는 스크립트(Card안에 스크립트를 직렬화하고, 모아주는 스크립트 필요할듯?)
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
        //설정후에 자신의 행동력을 텍스트에 표시하도록 (행동력 0을 감소시키도록 해서 자신의 초기행동력 표시)
        DecreaseEnergy(0);
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

    //쉴드 생성
    public void AddPlayerShield(int amount,int CardCost)
    {
        shield += amount;
        UpdateHpBar();
        //행동력 감소
        DecreaseEnergy(CardCost);
        //photonView.RPC(nameof(DecreaseEnergy), RpcTarget.AllBuffered, CardCost);
    }
    //공격 받음
    public void TakeDamage(int amount)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.AllBuffered, amount);
        //TakeDamageRPC(amount);
    }
    //행동력 감소 메서드 (RPC 실행용도)
    public void DecreaseEnergy(int cardCost)
    {
        photonView.RPC(nameof(DecreaseEnergyRPC), RpcTarget.AllBuffered, cardCost);
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
    //데미지 받았을때 모두에게 데미지 받은사람 알리기위해서
    [PunRPC]
    public void TakeDamageRPC(int amount)
    {
        CalcDamage(amount);
        UpdateHpBar();
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

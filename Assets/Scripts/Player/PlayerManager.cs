using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //플레이어의 HP.. 등등이 있어야한다 일단은 보류
    //플레이어 ID (포톤에서 불러와서 설정할 예정, 일단은 임시 아이디 넣자)
    public int PlayerID { get; private set; } = 1001;
    //플레이어의 덱
    PlayerDeck playerDeck;
    //플레이어 손패
    List<CardSO> playerHand = new List<CardSO>();

    //시작 플레이 카드
    [SerializeField,Tooltip("플레이어 시작 카드 수")] int startCardCount = 5;
    [SerializeField] RectTransform _cardSpawnPosition;
    [SerializeField] GameObject cardPrefab;

    //카드 내는 제한선
    [SerializeField] RectTransform useCardLine;


    //게임매니저에 자기자신을 설정해서 접근할수있도록
    private void OnEnable()
    {
        GameManager.Instance.SetPlayerManager(this);
    }
    private void OnDisable()
    {
        GameManager.Instance.DeletePlayerManager(this);
    }

    private void Start()
    {
        playerDeck = GetComponent<PlayerDeck>();
        //플레이어 초기 손패 5개 넣음
        for(int index = 0; index < startCardCount; index++)
        {
            SetPlayerHand();
        }
        Debug.Log(playerHand.Count);
    }
    public void SetPlayerHand()
    {
        //카드 데이터 가져오기
        CardSO cardSOData = playerDeck.GetCard();
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
        cardHover.Init(useCardLine);
        //다음에 뭔가 스크립트 짤때마다 여기서 계속 초기화
        CardArrorwUI arrorUI = cardView.GetComponent<CardArrorwUI>();
        arrorUI.Init();
        CardInputController cardInputController = cardView.GetComponent<CardInputController>();
        cardInputController.Init();
    }
}

using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MakeInitPlayerDeck:MonoBehaviour
{
    [SerializeField] List<CardDataTestSO> cardInitList;

    List<CardDataTestSO> currentPlayerDeck = new List<CardDataTestSO>();

    Dictionary<string, CardDataTestSO> cardInfoDic = new Dictionary<string, CardDataTestSO>();
    DatabaseReference dbRef = FirebaseAuthMgr.dbRef;
    DatabaseReference invenRef;
    DatabaseReference deckRef;

    FirebaseUser user = FirebaseAuthMgr.user;
    //카드를 UI에 표시하자
    [SerializeField] GameObject deckSettingPanel;
    [SerializeField] Transform deckParent;
    [SerializeField] Button closeButton;
    [SerializeField] CardInfoPrefab cardDeckPrefab;

    //저장 버튼 과 저장하기 위한 데이터 표시
    [SerializeField] Button saveButton;
    [SerializeField] GameObject inventoryScrollviewContent;
    [SerializeField] GameObject deckScrollviewContent;

    private void Awake()
    {
        invenRef = dbRef.Child("users").Child(user.UserId).Child("inven");
        deckRef = dbRef.Child("users").Child(user.UserId).Child("deck");
    }

    private void OnEnable()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
        saveButton.onClick.AddListener(()=> SaveDeckAndInventory());
    }
    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
    }

    private void CloseButtonClick()
    {
        //inventoryScrollviewContent
        //deckScrollviewContent
        //위에있는 2개의 자식들중에서 활성화된것들을 전부 ReturnPool해준다
        //1. 덱 객체들 풀로 다 보냄
        CardInfoPrefab[] deckPoolArray = deckScrollviewContent.GetComponentsInChildren<CardInfoPrefab>().Where(c => c.gameObject != deckScrollviewContent).ToArray();
        foreach (CardInfoPrefab prefab in deckPoolArray)
        {
            if (prefab.gameObject.activeInHierarchy)
            {
                DeckPoolSpawner.Instance.ReturnDeckCardToPool(prefab);
            }
        }
        //2. 인벤토리 객체들 풀로 다 보냄
        CardInfoPrefab[] invenPoolArray = inventoryScrollviewContent.GetComponentsInChildren<CardInfoPrefab>().Where(c => c.gameObject != inventoryScrollviewContent).ToArray();
        foreach(CardInfoPrefab prefab in invenPoolArray)
        {
            if(prefab.gameObject.activeInHierarchy)
            {
                DeckPoolSpawner.Instance.ReturnInvenCardToPool(prefab);
            }
        }
        deckSettingPanel.SetActive(false);
    }

    public void SaveDeckAndInventory()
    {
        List<string> invenCardList = new List<string>();
        List<string> deckCardList = new List<string>();
        //1. 인벤토리와 덱 2개의 스코롤뷰에서 정보를 가져와서 저장
        //자기자신을 빼고 자식들만 다 가져옴
        CardInfoPrefab[] invenPrefabs = inventoryScrollviewContent.GetComponentsInChildren<CardInfoPrefab>().Where(c => c.gameObject != inventoryScrollviewContent).ToArray();
        for(int index =0; index < invenPrefabs.Length; index++)
        {
            CardDataTestSO cardData = cardInfoDic[invenPrefabs[index].cardData.CardId];
            invenCardList.Add(cardData.CardId);
        }
        CardInfoPrefab[] deckPrefabs = deckScrollviewContent.GetComponentsInChildren<CardInfoPrefab>().Where(c => c.gameObject != deckScrollviewContent).ToArray();
        for(int index = 0; index < deckPrefabs.Length; index++)
        {
            CardDataTestSO cardData = cardInfoDic[deckPrefabs[index].cardData.CardId];
            deckCardList.Add(cardData.CardId);
        }
        //2. 현재 저장된 리스트를 Firebase에 저장한다.
        StartCoroutine(SaveDeckAndInventoryCor(invenCardList, deckCardList));
    }

    IEnumerator SaveDeckAndInventoryCor(List<string> invenCardList, List<string> deckCardList)
    {
        //DatabaseReference invenRef = dbRef.Child("users").Child(user.UserId).Child("inven");
        //DatabaseReference deckRef = dbRef.Child("users").Child(user.UserId).Child("deck");
        Task saveInvenTask = invenRef.SetValueAsync(invenCardList);
        yield return new WaitUntil(() => saveInvenTask.IsCompleted);
        Task saveDeckTask = deckRef.SetValueAsync(deckCardList);
        yield return new WaitUntil(() => saveDeckTask.IsCompleted);
        //예외처리
        if(saveInvenTask.Exception != null)
        {
            Debug.Log($"인벤토리 에서 저장이 실패했습니다 {saveInvenTask.Exception}");
        }
        if(saveDeckTask.Exception != null)
        {
            Debug.Log($"덱 에서 저장이 실패했습니다 {saveDeckTask.Exception}");
        }
    }

    public CardDataTestSO FindCardDataByID(string id)
    {
        if (!cardInfoDic.ContainsKey(id))
        {
            return null;
        }

        return cardInfoDic[id];
    }

    public async void OpenMyDeck()
    {
        //카드 데이터들을 설정
        //처음 덱설정 페이지를 여는것이었다면 카드별 정보를 딕셔너리에 캐싱해줌
        if (cardInfoDic.Count == 0)
        {
            InitCardData();
        }
        //유저가 가지고있는 덱 정보를 불러온다
        await LoadUserData();
    }
    public async Task MakeAndSaveInitPlayerDeck()
    {
        

        //카드 데이터가 들어있음.
        //1. 데이터 안에 들어있는 정보중에서 카드ID만 빼와서 리스트에 넣는다
        List<string> cardIdList = new List<string>();
        foreach(CardDataTestSO item in cardInitList)
        {
            cardIdList.Add(item.CardId);
        }
        Task task = deckRef.SetValueAsync(cardIdList);
        //yield return new WaitUntil(() => task.IsCompleted);
        Debug.Log("초기덱 저장완료");
        await task;
    }

    

    public void InitCardData()
    {
        //초기 딕셔너리 데이터들을 초기화
        //CardDataSO[] allCards = Resources.LoadAll<CardDataSO>("Cards"); ;
        CardDataTestSO[] allCards = Resources.LoadAll<CardDataTestSO>("Cards");
        foreach(CardDataTestSO cardData in allCards)
        {
            cardInfoDic.Add(cardData.CardId, cardData);
        }
    }

    //읽기방법. 데이터를 읽는다 (코루틴으로 불러오고 성공여부 파악. 하위에 있는 것들을 .Child를 통해 접근(리스트,딕셔너리 가능)
    private async Task LoadUserData()
    {
        //코루틴 yield return 방식을 async/ await 비동기 방식으로 바꿈 -> DB는 이게 더 좋다고함
        Task<DataSnapshot> deckTask = deckRef.GetValueAsync(); //유저 아이디 속의 모든 정보를 가져옴
        Task<DataSnapshot> invenTask = invenRef.GetValueAsync();
        await Task.WhenAll(deckTask, invenTask); //전부 끝날때까지 기다림
        //yield return new WaitUntil(() => DBTask.IsCompleted); 

        if (deckTask.Exception != null || invenTask.Exception != null) //에러가 발생했다면
        {
            Debug.LogWarning($"데이터 불러오기 실패 {deckTask.Exception} / {invenTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = deckTask.Result; //결과는 덩어리로 기억되있으니, 이걸 나중에 분석하려고 큰틀에 담아둠

            if (deckTask.Result.Value == null) //데이터없는 사람도 체크 (한번도 저장안한사람,즉 처음 게임을 플레이중인사람)
            {
                //초기 덱을 설정해주자 (코루틴안에서 코루틴을 실행해야할때는 async await을 쓴다)
                Task saveTask = MakeAndSaveInitPlayerDeck();
                //다시 불러온다
                Debug.Log("초기덱 저장완료2");
                //yield return new WaitUntil(() => saveTask.IsCompleted);
                await saveTask;
                deckTask = deckRef.GetValueAsync();
                //yield return new WaitUntil(() => deckTask.IsCompleted);
                await deckTask;
            }
            Debug.Log("초기덱 저장완료3");
            currentPlayerDeck.Clear();
            deckSettingPanel.SetActive(true);
            //플레이어 덱 리스트 DB에서 가져오기
            foreach (DataSnapshot item in deckTask.Result.Children) //Child는 하나, Children은 여러개
            {
                string cardId = (item.Value.ToString());
                //string값을 가져왔다. 이거를 이제 딕셔너리를 이용해서 덱을 설정해주자
                currentPlayerDeck.Add(cardInfoDic[cardId]);
                //Instantiate(cardInfoDic[cardId], deckParent);
                //이제 정보들로 카드를 생성해준다
                //CardInfoPrefab info = Instantiate(cardDeckPrefab, deckParent);
                CardInfoPrefab info = DeckPoolSpawner.Instance.GetDeckCardByPool();
                info.SetCardData(cardInfoDic[cardId]);
            }
            //플레이어의 인벤토리의 카드 리스트도 DB에서 가져오기
            foreach(DataSnapshot item in invenTask.Result.Children)
            {
                string cardId = item.Value.ToString();
                CardInfoPrefab info = DeckPoolSpawner.Instance.GetInventoryCardByPool();
                info.SetCardData(cardInfoDic[cardId]);
            }
        }
    }
}

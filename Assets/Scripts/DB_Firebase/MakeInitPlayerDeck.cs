using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MakeInitPlayerDeck : MonoBehaviour
{
    [SerializeField] List<CardDataTestSO> cardInitList;

    List<CardDataTestSO> currentPlayerDeck = new List<CardDataTestSO>();

    Dictionary<string, CardDataTestSO> cardInfoDic = new Dictionary<string, CardDataTestSO>();
    DatabaseReference dbRef = FirebaseAuthMgr.dbRef;
    FirebaseUser user = FirebaseAuthMgr.user;
    //카드를 UI에 표시하자
    [SerializeField] GameObject deckSettingPanel;
    [SerializeField] Transform deckParent;
    [SerializeField] Button closeButton;
    [SerializeField] CardInfoPrefab cardDeckPrefab;

    private void OnEnable()
    {
        closeButton.onClick.AddListener(() => deckSettingPanel.SetActive(false));
    }
    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
    }

    public void OpenMyDeck()
    {
        //카드 데이터들을 설정
        //처음 덱설정 페이지를 여는것이었다면 카드별 정보를 딕셔너리에 캐싱해줌
        if (cardInfoDic.Count == 0)
        {
            InitCardData();
        }
        //유저가 가지고있는 덱 정보를 불러온다
        StartCoroutine(LoadUserData());
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
        Task task = dbRef.Child("users").Child(user.UserId).Child("deck").SetValueAsync(cardIdList);
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
    private IEnumerator LoadUserData()
    {
        Task<DataSnapshot> DBTask = dbRef.Child("users").Child(user.UserId).Child("deck").GetValueAsync(); //유저 아이디 속의 모든 정보를 가져옴
        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null) //에러가 발생했다면
        {
            Debug.LogWarning($"데이터 불러오기 실패 {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result; //결과는 덩어리로 기억되있으니, 이걸 나중에 분석하려고 큰틀에 담아둠

            List<string> cardIdName = new List<string>();

            if (DBTask.Result.Value == null) //데이터없는 사람도 체크 (한번도 저장안한사람,즉 처음 게임을 플레이중인사람)
            {
                //초기 덱을 설정해주자 (코루틴안에서 코루틴을 실행해야할때는 async await을 쓴다)
                Task saveTask = MakeAndSaveInitPlayerDeck();
                //다시 불러온다
                Debug.Log("초기덱 저장완료2");
                yield return new WaitUntil(() => saveTask.IsCompleted);
                DBTask = dbRef.Child("users").Child(user.UserId).Child("deck").GetValueAsync();
                yield return new WaitUntil(() => DBTask.IsCompleted);
            }
            Debug.Log("초기덱 저장완료3");
            currentPlayerDeck.Clear();
            deckSettingPanel.SetActive(true);
            //플레이어 덱 리스트 DB에서 가져오기
            foreach (DataSnapshot item in DBTask.Result.Children) //Child는 하나, Children은 여러개
            {
                string cardId = (item.Value.ToString());
                //string값을 가져왔다. 이거를 이제 딕셔너리를 이용해서 덱을 설정해주자
                currentPlayerDeck.Add(cardInfoDic[cardId]);
                //Instantiate(cardInfoDic[cardId], deckParent);
                //이제 정보들로 카드를 생성해준다
                CardInfoPrefab info = Instantiate(cardDeckPrefab, deckParent);
                info.SetCardData(cardInfoDic[cardId]);
            }
            // Debug.Log($"인벤토리 로드 완료 : {string.Join(", ", inventory)}");
            //아래부터는 가져온 덱 리스트를 가지고 이용하면 된다.
            //아마 가지고있는 카드 (인벤토리 리스트)도 필요할듯. 가지고있는 카드를 드래그하면 현재 카드덱 카드와 교체되고, 쓰는 방식(저장도 해야할거임)
            //1. 현재 카드를 UI에 표시해보자
            //deckSettingPanel.SetActive(true);
            int kk = 20;
            //딕셔너리에 캐싱해줘서 사용(SO기반으로)-> SO에 어떤 이미지 쓸지도 해놔서 이미지 긁어서 넣으면 된다.


            //2.왼쪽에 인벤토리로 현재 보유중인 카드를 보여주자

            //3. 보유중인 카드를 덱의 카드와 교체할수있게. 저장하면 DB에 저장
        }
    }
}

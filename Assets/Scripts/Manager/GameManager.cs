using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    //턴 관리 매니저
    public TurnManager turnManager { get; private set; }
    //현재 턴에서 어떤 플레이어가 진행중인지
    public PlayerManager playerManager { get; private set; }
    //카드를 사용할때 화살표UI를 만들기위해서
    public TargetingManager targetingManager { get; private set; }
    //리소스를 관리하기위한 매니저
    public AssetManager assetManager { get; private set; }
    //네트워크를 관리하는 매니저
    public InGameNetworkMgr inGameNetworkMgr { get; private set; }
    //게임결과 패널을 관리하는 매니저
    [SerializeField] private GameResultPanelManager gameResultPanelManager;

    //플레이어 ID
    string[] playerIdArray;
    //플레이어가 소환될위치 (플레이어1, 플레이어2)
    [SerializeField] GameObject playerSpawnPosParent;
    Transform[] spawnPositions;
    //네트워크 객체로 소환될 플레이어 프리팹
    [SerializeField] GameObject playerPrefab;
    //자신의 카드들 소환될위치와 카드 프리팹
    [SerializeField] RectTransform _cardSpawnPosition;
    [SerializeField] GameObject _cardPrefab;
    //카드 내는 제한선
    [SerializeField] RectTransform _useCardLine;
    
    //카드ID가 string으로 주어졌을때, 어떤 카드가 매칭되어야하는지 캐싱한 딕셔너리
    Dictionary<string, CardSO> cardSODic = new Dictionary<string, CardSO>();
    public IReadOnlyDictionary<string, CardSO> CardSODic => cardSODic;
    //Firebase DB에서 불러온 자신의 원본덱 목록들
    List<CardSO> deckCardList = new List<CardSO>();

    //플레이어 레벨 정보
    PlayerLevelData playerLevelData;

    [SerializeField] public TextMeshProUGUI _energyText;
    //현재 방의 플레이어 인원 (관전자 제외)
    public int maxPlayerCount { get; private set; }
    //플레이어별 어떤 네트워크 객체를 사용하고있는지 (클래스안에 UI객체 넣을때 필요해졌어서 정의했음)
    Dictionary<int, PlayerManager> _playerInstanceDic = new Dictionary<int, PlayerManager>();
    public IReadOnlyDictionary<int, PlayerManager> PlayerInstanceDic => _playerInstanceDic;

    //플레이어의 상태이상별 어떤 이미지를 가지고있어야하는지
    private Dictionary<eConditionType, Sprite> _playerConditionImageDic = new Dictionary<eConditionType, Sprite>();

    //생성위치에 따라 특정 캐릭터 모델을 사용하도록 처리
    [SerializeField] List<GameObject> _characterModelPrefabs;
    public IReadOnlyList<GameObject> CharacterModelPrefabs => _characterModelPrefabs;

    [SerializeField] AudioClip _inGamebgm;
    protected override void Awake()
    {
        isDestroyOnLoad = false;
        base.Awake();

        //초기 설정
        //플레이어 갯수 구함
        _playerInstanceDic.Clear();
        CalcPlayerCount();
        //플레이어가 가질수있는 상태이상의 아이콘 이미지 설정
        _playerConditionImageDic.Clear();
        InitPlayerConditionDic();
    }

    private IEnumerator Start()
    {
        yield return null;
        //BGM재생
        SoundManager.Instance.PlayBackgroundMusic(_inGamebgm);
        //자신의 카드를 DB에서 불러와서 설정하자.
        //1. 카드의 전체 정보를 저장하고있는 딕셔너리가 있어야한다. 
        //2. DB-덱에서 자신의 카드 정보를 불러온다. (무조건 있다고 가정.)
        //3. 카드 딕셔너리에서 string형식으로된 id를 읽어서 하나씩 리스트로 만들어준다.
        //4. 덱 셔플? 그 알고리즘으로 카드를 하나씩 받는다.

        //카드의 전체 정보를 저장하고있는 딕셔너리 생성
        spawnPositions = playerSpawnPosParent.GetComponentsInChildren<Transform>().Where(c => c.gameObject != playerSpawnPosParent).ToArray();
        CardSO[] cards = Resources.LoadAll<CardSO>("CardData");
        foreach(CardSO item in cards)
        {
            cardSODic[item.CardId] = item;
        }
        // DB에서 자신의 카드 정보를 불러옴
        StartCoroutine(MakeAndSetPlayerInfo());
        //3. 부터 해야함
        //플레이어를 생성하고, 그 플레이어의 덱을 설정해주자.
        //생성위치 지정.
        

        //임시용 (포톤 적용전까지는 무조건 내턴이도록 하자)
        //turnManager.SetPlayerID(playerManager.PlayerID);
        //StartCoroutine(SetInitPlayerTurn());
    }
    IEnumerator MakeAndSetPlayerInfo()
    {
        //현재 자신의 초기덱이 설정완료될때까지 기다림
        yield return StartCoroutine(LoadDeckData());

        //현재 플레이어의 레벨정보를 불러옴
        yield return StartCoroutine(LoadPlayerData());

        //플레이어 생성 및 초기 설정
        Player ply = PhotonNetwork.LocalPlayer;
        bool isPlayer = (bool)ply.CustomProperties[NetworkEventManager.IsPlayer];
        if (isPlayer)
        {
            int spawnIndex = (int)ply.CustomProperties[NetworkEventManager.SeatIndex];
            playerManager = PhotonNetwork.Instantiate(playerPrefab.name, spawnPositions[spawnIndex].position, Quaternion.identity).GetComponent<PlayerManager>();
            //매개변수 1개로만 쓸수있도록 클래스 생성해서 하나로 담음(매개변수 순서 안지켜도됨)
            PlayerConfig config = new PlayerConfig
            {
                cardSpawnPosition = _cardSpawnPosition,
                cardPrefab = _cardPrefab,
                useCardLine = _useCardLine,
                id = ply.ActorNumber,
                originDeck = deckCardList,
                levelData = playerLevelData
            };

            //개인만 가지고있어야하는 설정을 Init로 만들음
            playerManager.Init(config);
            //모두가 네트워크에 보여야하는것들 설정을 위해 RPC로 보냄
            playerManager.photonView.RPC(nameof(playerManager.RPC_Init), RpcTarget.AllBuffered, playerLevelData.Level,playerLevelData.Exp, ply.ActorNumber);
            //생성 위치에 따라서 고정된 캐릭터 모델을 사용.
            playerManager.photonView.RPC(nameof(playerManager.RPC_InitCharacterModel), RpcTarget.AllBuffered, spawnIndex);

            int Num = GameManager.Instance.playerManager.PlayerID;
            Debug.Log($"{Num}");
        }
    }
    //실제 플레이어가 몇명인지 확인 (플레이어가 모두 설정될때까지 대기하기 위해서)
    public void CalcPlayerCount()
    {
        maxPlayerCount = 0;
        foreach (Player ply in PhotonNetwork.PlayerList)
        {
            //플레이어라면?
            if ((bool)ply.CustomProperties[NetworkEventManager.IsPlayer])
            {
                maxPlayerCount++;
            }
        }
    }

    //플레이어가 가질수있는 상태이상의 종류별로 어떤 이미지를 표시해야하는지 딕셔너리 캐싱
    public void InitPlayerConditionDic()
    {
        PlayerConditionTypeSO[] conditionSOArray = Resources.LoadAll<PlayerConditionTypeSO>("ConditionData").ToArray();
        //enum값이 주어졌을때 Sprite를 반환하는 딕셔너리
        foreach(PlayerConditionTypeSO condition in conditionSOArray)
        {
            _playerConditionImageDic.Add(condition.ConditionType, condition.Image);
        }
    }
    public Sprite GetConditionSprite(eConditionType type)
    {
        if (_playerConditionImageDic.ContainsKey(type))
        {
            return _playerConditionImageDic[type];
        }
        else
        {
            return null;
        }
    }


    //초기에 PlayerStatus를 불러옴. 그런데 만약에 정보를 불러올수없다면?
    private IEnumerator LoadPlayerData()
    {
        DatabaseReference playerRef = NetworkEventManager.Instance.GetPlayerRef();
        Task<DataSnapshot> DBTask = playerRef.GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);
        if (DBTask.Exception != null) //에러가 발생했다면
        {
            Debug.LogWarning($"데이터 불러오기 실패 {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null) //데이터없는 사람도 체크 (한번도 저장안한사람)
        {
            Debug.LogWarning("저장된 데이터가 없습니다");
            //새로 만들어주자
            PlayerLevelData playerData = new PlayerLevelData();
            string newJson = JsonUtility.ToJson(playerData);
            Task saveTask = playerRef.SetRawJsonValueAsync(newJson);
            yield return new WaitUntil(() => saveTask.IsCompleted);
            if(saveTask.Exception != null)
            {
                Debug.LogWarning($"플레이어 데이터 저장 실패 {saveTask.Exception}");
            }
            //새로 만들어준 데이터로 플레이어 정보 설정
            playerLevelData = playerData;
            yield break;
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            string json = snapshot.GetRawJsonValue();
            Debug.Log($"Load json 데이터: {json}");
            playerLevelData = JsonUtility.FromJson<PlayerLevelData>(json);
            Debug.Log("플레이어 데이터 저장완료");
        }
    }

    //플레이어 데이터 저장. 불러오기는 코루틴써봤고, 저장은 async 써봤음. async는 한프레임내에서 끝나면 바로 시작한다고함.
    public async Task SavePlayerData(int expAddValue)
    {
        Debug.Log($"플레이어 경험치를 증가시키겠습니다 {expAddValue}");
        //플레이어 경험치량을 통해 레벨업 계산
        playerLevelData.AddExpAndCheckLevelUp(expAddValue);
        Debug.Log($"저장전{FirebaseAuth.DefaultInstance.CurrentUser}의 플레이어 레벨: {playerLevelData.Level} 경험처: {playerLevelData.Exp} 저장됨 ");
        //DB에 결과값 저장
        string json = JsonUtility.ToJson(playerLevelData);
        DatabaseReference playerDataRef = NetworkEventManager.Instance.GetPlayerRef();

        try
        {
            await playerDataRef.SetRawJsonValueAsync(json);
            Debug.Log($"저장후{FirebaseAuth.DefaultInstance.CurrentUser}의 플레이어 레벨: {playerLevelData.Level} 경험처: {playerLevelData.Exp} 저장됨 ");
        }
        catch(Exception ex)
        {
            Debug.Log($"저장에 예외발생 {ex}");
        }
 
    }

    private IEnumerator LoadDeckData()
    {
        DatabaseReference deckRef = NetworkEventManager.Instance.GetDeckRef();
        Task<DataSnapshot> DBTask = deckRef.GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null) //에러가 발생했다면
        {
            Debug.LogWarning($"데이터 불러오기 실패 {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null) //데이터없는 사람도 체크 (한번도 저장안한사람)
        {
            Debug.LogWarning("저장된 데이터가 없습니다");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result; //결과는 덩어리로 기억되있으니, 이걸 나중에 분석하려고 큰틀에 담아둠
            //리스트 가져오기
            deckCardList.Clear(); //기존 로컬상의 인벤토리 날리고 
            foreach (DataSnapshot item in DBTask.Result.Children) //Child는 하나, Children은 여러개
            {
                CardSO currentCard = cardSODic[item.Value.ToString()];
                deckCardList.Add(currentCard);
                //만약 부하를 여러 프레임에 분산시키려면 yield로 할수도있지만, 작업도중 Save가 되면 이상해질수있음. DB는 무결성이 중요하기때문에 안하는게 좋음
            }
            Debug.Log("DB에서 덱 데이터 Load 완료");
        }

    }
    //게임결과창 열기 
    public void ShowGameResultPanel(string gameResult, string resultInfo )
    {
        bool isPlayer = false;
        //모든 플레이어 돌면서 이게 자기꺼면 자신이 플레이어라는것을 확인
        foreach(int actorNum in _playerInstanceDic.Keys)
        {
            if(_playerInstanceDic[actorNum].photonView.IsMine) { isPlayer = true; } 
        }
        gameResultPanelManager.ActiveResultPanelAndSetInfo(gameResult, resultInfo, isPlayer);
    }
    //자신이 게임 플레이어인지 확인하는 메서드
    public bool CheckPlayerAble(int actorNumber)
    {
        if(_playerInstanceDic.ContainsKey(actorNumber)) { return true; }
        else { return false; }
    }

    #region GameManager에 있는 매니저들을 설정하고, 할당해제하기위한 메서드
    //턴매니저 할당&해제
    public void SetTurnManager(TurnManager turnMgr)
    {
        turnManager = turnMgr;
    }
    public void DeleteTurnManager(TurnManager turnMgr)
    {
        turnManager = null;
    }
    //타겟팅 매니저 할당&해제
    public void SetTargetingManager(TargetingManager targetMgr)
    {
        targetingManager = targetMgr;
    }
    public void DeleteTargetManager(TargetingManager targetMgr)
    {
        targetingManager = null;
    }

    public bool IsPlayerInstantiateComplete()
    {
        //일단 로직상 같아야함.
        if (_playerInstanceDic.Count == maxPlayerCount) return true;
        else return false;
    }
    public void SetPlayerManager(PlayerManager playerMgr,int actorID)
    {
        if(!_playerInstanceDic.ContainsKey(actorID))
        {
            _playerInstanceDic[actorID] = playerMgr;
            Debug.Log($"{actorID}등록되었습니다");
        }
        //플레이어가 사망했을때 이벤트가 등록되어야 하므로 추가
        playerMgr.OnPlayerDead += inGameNetworkMgr.PlayerDeadNotified;
    }
    public void DeletePlayerManager(PlayerManager playerMgr,int actorID)
    {
        //playerManager = null;
        if(_playerInstanceDic.ContainsKey(actorID))
        {
            _playerInstanceDic.Remove(actorID);
        }
    }
    public void SetAssetManager(AssetManager assetMgr)
    {
        assetManager = assetMgr;
    }
    public void DeleteAssetManager(AssetManager assetMgr)
    {
        assetManager = null;
    }
    public void SetInGameNetworkManager(InGameNetworkMgr networkMgr)
    {
        inGameNetworkMgr = networkMgr;
    }
    public void DeleteInGameNetworkManager(InGameNetworkMgr networkMgr)
    {
        inGameNetworkMgr = null;
    }
    public void SetGameResultPanelManager(GameResultPanelManager gameResultPanelMgr)
    {
        gameResultPanelManager = gameResultPanelMgr;
    }
    public void DeleteGameResultPanelManager(GameResultPanelManager gameResultPanelMgr)
    {
        gameResultPanelManager = null;
    }

    #endregion

}

using Firebase.Auth;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public TurnManager turnManager { get; private set; }
    public PlayerManager playerManager { get; private set; }

    public TargetingManager targetingManager { get; private set; }

    //플레이어 ID
    string[] playerIdArray;
    [SerializeField] GameObject playerSpawnPosParent;
    Transform[] spawnPositions;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] RectTransform _cardSpawnPosition;
    [SerializeField] GameObject _cardPrefab;
    //카드 내는 제한선
    [SerializeField] RectTransform _useCardLine;
    protected override void Awake()
    {
        isDestroyOnLoad = false;
        base.Awake();
    }

    private IEnumerator Start()
    {
        //임시용 (플레이어 아이디 설정하기위해서
        yield return null;
        //임시용 (포톤 적용전까지는 무조건 내턴이도록 하자)
        //turnManager.SetPlayerID(playerManager.PlayerID);
        StartCoroutine(SetInitPlayerTurn());
    }

    IEnumerator SetInitPlayerTurn()
    {
        yield return new WaitUntil(() => turnManager != null);
        //플레이어 설정
        playerIdArray = NetworkEventManager.Instance.GetPlayerID();
        spawnPositions = playerSpawnPosParent.GetComponentsInChildren<Transform>().Where(c => c.gameObject != playerSpawnPosParent).ToArray();

        turnManager.SetPlayerID(playerIdArray[0]);

        for(int index =0; index < playerIdArray.Length; index++)
        {
            if(FirebaseAuth.DefaultInstance.CurrentUser.UserId == playerIdArray[index])
            {
                SpawnPlayer(index, FirebaseAuth.DefaultInstance.CurrentUser.UserId);
            }
        }
    }
    public void SpawnPlayer(int index, string id)
    {
        //네트워크 객체 소환. 네트워크 객체는 오브젝트풀 사용안해야함
        playerManager = PhotonNetwork.Instantiate(playerPrefab.name, spawnPositions[index].position, Quaternion.identity).GetComponent<PlayerManager>();
        playerManager.Init(_cardSpawnPosition, _cardPrefab, _useCardLine, id);

    }
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
}

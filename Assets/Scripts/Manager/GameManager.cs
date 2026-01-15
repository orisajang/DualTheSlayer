using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public TurnManager turnManager { get; private set; }
    public PlayerManager playerManager { get; private set; }

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
        turnManager.SetPlayerID(playerManager.PlayerID);
    }

    //턴매니저 할당&해제
    public void SetTurnManager(TurnManager turnMgr)
    {
        turnManager = turnMgr;
    }
    public void DeleteTurnManager()
    {
        turnManager = null;
    }
    //플레이어매니저 할당&해제
    public void SetPlayerManager(PlayerManager playerMgr)
    {
        playerManager = playerMgr;
    }
    public void DeletePlayerManager()
    {
        playerManager = null;
    }
}

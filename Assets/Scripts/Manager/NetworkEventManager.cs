using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkEventManager : MonoBehaviourPunCallbacks
{
    //방을 떠나고 로비를 입장할때 등 씬 이동 시 계속 살아있지않으면 이벤트를 받을 수없는 상황에 사용하는 매니저
    public static NetworkEventManager Instance { get; private set; }

    //네트워크 전체에서 쓰는 고정된 이름들
    public const string GamePlayerId = "GamePlayerID";
    public const string PLAYER_SEATS = "Player";
    public const string SPECTOR_SEATS = "Spector";
    public const string IsPlayer = "IsPlayer";
    public const string SeatIndex = "SeatIndex";
    


    //전역에서 쓰는 덱 주소경로
    public DatabaseReference GetDeckRef()
    {
        FirebaseUser user = FirebaseAuthMgr.user;
        return FirebaseAuthMgr.dbRef.Child("users").Child(user.UserId).Child("deck");
    }
    //전역에서 쓰는 인벤토리 주소 경로
    public DatabaseReference GetInvenRef()
    {
        FirebaseUser user = FirebaseAuthMgr.user;
        return FirebaseAuthMgr.dbRef.Child("users").Child(user.UserId).Child("inven");
    }
    //전역에서 쓰는 플레이어 주소 경로
    public DatabaseReference GetPlayerRef()
    {
        FirebaseUser user = FirebaseAuthMgr.user;
        return FirebaseAuthMgr.dbRef.Child("users").Child(user.UserId).Child("playerData");
    }

    //게임할 플레이어아이디 저장
    private string[] playerIdArray;
    public string[] GetPlayerID()
    {
        Room room = PhotonNetwork.CurrentRoom;
        playerIdArray = (string[])room.CustomProperties[GamePlayerId];

        return playerIdArray; //이거아님
    }

    private void Awake()
    {
        // 싱글톤이랑 MonoBehaviourPunCallbacks 랑 둘다 같이 상속받을수없어서 그냥 여기다가 싱글톤 코드 넣음
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; //이게 없으면 자식의 Awake가 끝까지 실행됨
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }
    public override void OnLeftLobby()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("완료");
    }
}

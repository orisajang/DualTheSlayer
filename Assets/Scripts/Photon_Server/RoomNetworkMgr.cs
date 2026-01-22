using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomNetworkMgr : MonoBehaviourPunCallbacks
{
    [SerializeField] Button _gameStartButton;
    [SerializeField] Button _roomLeaveButton;

    [SerializeField] RoomTestView roomTestview;

    private void Awake()
    {
        _gameStartButton.onClick.AddListener(GameStart);
        _roomLeaveButton.onClick.AddListener(LeaveRoom);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        UIRoomManager.Instance.SetRoomNetworkManager(this);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        if(UIRoomManager.isHaveInstance) UIRoomManager.Instance.RemoveRoomNetworkManager(this);
    }
    private void OnDestroy()
    {
        _gameStartButton.onClick.RemoveAllListeners();
        _roomLeaveButton.onClick.RemoveAllListeners();
    }

    //게임 시작 버튼을 누르면 특정 씬으로 이동해야한다.
    private void GameStart()
    {
        IReadOnlyList<Seats> playerInfo = roomTestview.GetPlayerSetInfo();


        //네트워크 프로퍼티로 저장해야할듯. 불러와야함
        Room room = PhotonNetwork.CurrentRoom;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        string[] playerArray = new string[playerInfo.Count];
        for(int index=0; index< playerArray.Length; index++)
        {
            playerArray[index] = playerInfo[index].playerId;
        }
        hash.Add(NetworkEventManager.GamePlayerId, playerArray);
        room.SetCustomProperties(hash);
    }
    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName}님이 입장했습니다");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName}님이 나갔습니다");
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"{newMasterClient.NickName}님이 방장이 되었습니다");
        //방장이 할 행동을 해야함. (시작버튼 활성화 등)
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //커스텀 프로퍼티를 통해 룸 정보 변경되었다면 정리하자
        if (propertiesThatChanged.ContainsKey(NetworkEventManager.PLAYER_SEATS) || propertiesThatChanged.ContainsKey(NetworkEventManager.SPECTOR_SEATS))
        {
            roomTestview.RoomTest.LoadSeatRoomData();
            roomTestview.UIUpdateByData();
        }
        
        if(propertiesThatChanged.ContainsKey(NetworkEventManager.GamePlayerId))
        {
            PhotonNetwork.LoadLevel("InGameScene");
        }
    }
}

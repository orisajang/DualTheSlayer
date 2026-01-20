using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
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
        PhotonNetwork.LoadLevel("InGameScene");
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
        //roomTestview.RoomTest.LoadSeatsFromRoom();
        //roomTestview.UIUpdate();
        roomTestview.RoomTest.LoadSeatRoomData();
        roomTestview.UIUpdateByData();

        //커스텀 프로퍼티 정리하자
    }
}

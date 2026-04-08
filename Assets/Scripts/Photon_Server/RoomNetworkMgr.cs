using Firebase.Auth;
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
        //Room room = PhotonNetwork.CurrentRoom;
        //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        //string[] playerArray = new string[playerInfo.Count];
        //for(int index=0; index< playerArray.Length; index++)
        //{
        //    playerArray[index] = playerInfo[index].playerId;
        //}
        //hash.Add(NetworkEventManager.GamePlayerId, playerArray);
        //room.SetCustomProperties(hash);

        //게임 시작 버튼이 눌린다-> 현재 자신이 플레이어인지 관전자인지 정보를 설정한다
        //버튼을 누른 사람만 설정하지 않고, 모두가 설정을 해야하기 때문에 RPC로 설정한다
        photonView.RPC(nameof(SetPlayerTypeProperty), RpcTarget.All);
    }
    [PunRPC]
    private void SetPlayerTypeProperty()
    {
        //자신이 게임플레이어인지, 관전자인지 체크해서 프로퍼티를 설정한다.
        //커스텀프로퍼티는 무조건 해쉬
        ExitGames.Client.Photon.Hashtable playerProperty = new ExitGames.Client.Photon.Hashtable();
        string currentUserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        //현재 자신의 타입을 가져옴
        Seats userSeat = roomTestview.RoomTest.ReturnPlayerType(currentUserId);
        bool isPlayer = (userSeat.seatType == eSeatType.Player) ? true : false;
        //프로퍼티 설정 에서 리턴 이벤트 받아서 처리
        playerProperty[NetworkEventManager.IsPlayer] = isPlayer;
        playerProperty[NetworkEventManager.SeatIndex] = userSeat.seatIndex;
        Player player = PhotonNetwork.LocalPlayer;
        player.SetCustomProperties(playerProperty);
    }


    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Debug.Log($"{newPlayer.NickName}님이 입장했습니다");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Debug.Log($"{otherPlayer.NickName}님이 나갔습니다");
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //Debug.Log($"{newMasterClient.NickName}님이 방장이 되었습니다");
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
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(changedProps.ContainsKey(NetworkEventManager.IsPlayer))
        {
            //자신이 플레이어인지, 관전자인지 설정한 이벤트 (1회만 수행해야하므로 마스터가 진행)
            //전체 플레이어를 확인하면서 전체 플레이어가 전부 다 커스텀프로퍼티 값이 있는지 확인한 후에, 룸 이동
            if(PhotonNetwork.IsMasterClient)
            {
                bool isAllReady = true;
                foreach (Player ply in PhotonNetwork.PlayerList)
                {
                    if (!ply.CustomProperties.ContainsKey(NetworkEventManager.IsPlayer))
                    {
                        isAllReady = false;
                        break;
                    }
                }
                if (isAllReady)
                {
                    //전체 플레이어가 설정이 완료되었으므로 다음 씬으로 이동하는 기능 사용
                    PhotonNetwork.LoadLevel("InGameScene");
                }
            }
        }
    }
}

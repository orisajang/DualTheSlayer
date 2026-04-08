using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class LobbyNetworkMgr : MonoBehaviourPunCallbacks
{
    [SerializeField] Button _joinRandomRoomButton;
    //방 목록을 불러왔을때 UI에 표시하기 위해서
    [SerializeField] GameObject _roomButton;
    [SerializeField] Transform _roomListParent;

    //방 입장 실패했을때 띄울 패널
    [SerializeField] GameObject errorPanel;
    RoomEnterFailScript _failScript;


    //딕셔너리 캐싱을 통해 방이 바뀌었을때 정보를 설정
    Dictionary<string, GameObject> _roomInfoDic = new Dictionary<string, GameObject>();


    private void Awake()
    {
        _joinRandomRoomButton.onClick.AddListener(CreateRandomRoom);
        _failScript = errorPanel.GetComponent<RoomEnterFailScript>();
    }
    private void Start()
    {
        StartCoroutine(EnterLobbyCoroutine());
    }

    private IEnumerator EnterLobbyCoroutine()
    {
        yield return new WaitUntil(() => (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InLobby));
        PhotonNetwork.JoinLobby();
    }

    private void OnDestroy()
    {
        _joinRandomRoomButton.onClick.RemoveAllListeners();
    }
    public override void OnEnable()
    {
        //UI매니저에 자신을 등록해서 접근 가능하도록 처리
        base.OnEnable();
        UILobbyManager.Instance.SetLobbyManager(this);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        if(UILobbyManager.isHaveInstance) UILobbyManager.Instance.DeleteLobbyManager(this);
    }
    //로비 화면 만들기 (버튼 누르면 룸화면으로 이동하도록)
    public void CreateRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    //방 제목을 입력해서 방 만들기
    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 8,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.CreateRoom(roomName, options);
    }

    //방 입장 실패
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //Debug.Log(returnCode + " " + message); //혹은 팝업 띄울수있다
        _failScript.Init($"입장실패. {returnCode} {message}");
        errorPanel.SetActive(true);
        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 }); //실패했다면 방 생성한다
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log($"랜덤방 입장 실패");
        _failScript.Init("입장할 수 있는 방이 없습니다. 새로 생성해주세요");
        errorPanel.SetActive(true);
        
    }
    //방 입장 성공
    public override void OnJoinedRoom()
    {
        //Debug.Log("룸으로 이동합니다");
        Room room = PhotonNetwork.CurrentRoom;
        //Debug.Log($"{room.Name} 방에 {PhotonNetwork.NickName}님이 입장함"); //방이름, 닉네임
        //(추가필요) 이후에 원한다면 커스텀 프로퍼티로 플레이어의 HP값들을 설정해야함 (늦게 들어온 사람도 HP값이 몇인지 받을수있도록) -> 인게임에서 하는건가?
        SceneManager.LoadScene(2);
    }
    public void JoinRoomByName(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Debug.Log("룸리스트 업데이트");
        //방 정보가 업데이트 되거나, JoinLobby성공시 한번
        foreach (RoomInfo roomInfo in roomList)
        {
            //삭제된 방
            if(roomInfo.RemovedFromList)
            {
                if(_roomInfoDic.TryGetValue(roomInfo.Name,out GameObject roomButton))
                {
                    Destroy(roomButton);
                    _roomInfoDic.Remove(roomInfo.Name);
                    continue;
                }
            }
            //새 방
            if(!_roomInfoDic.ContainsKey(roomInfo.Name))
            {
                //버튼을 1개씩 생성해주고, 버튼누르면 해당 방에 입장할 수 있도록 Init
                GameObject room = Instantiate(_roomButton, _roomListParent);
                RoomButtonScript script = room.GetComponent<RoomButtonScript>();
                script.Init(this, roomInfo.Name, roomInfo.PlayerCount.ToString(), roomInfo.MaxPlayers.ToString());
                _roomInfoDic.Add(roomInfo.Name, room);
            }
            else
            {
                //기존에 있는 방
                RoomButtonScript script = _roomInfoDic[roomInfo.Name].GetComponent<RoomButtonScript>();
                script.UpdateRoomInfo(roomInfo.Name, roomInfo.PlayerCount.ToString(), roomInfo.MaxPlayers.ToString());
            }


            
        }
    }
}
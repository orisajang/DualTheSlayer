using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class TitleMgr : MonoBehaviourPunCallbacks
{
    //포톤 연결될거같으면 무조건 MonoBehaviourPunCallbacks을 쓰자
    //초기 서버 연결
    //버튼 누르면 다음 (로비로 이동 및 씬도 이동)
    string gameVersion = "1.0";

    [SerializeField] Button goLobbyButton;
    [SerializeField] AudioClip _titelSound;

    private void Awake()
    {
        //포톤 초기 설정
        PhotonNetwork.AutomaticallySyncScene = true; //방장이 씬을 바꾸면 다른 모든 클라이언트들도 씬이 같이 바뀌는 옵션
        PhotonNetwork.GameVersion = gameVersion; //시작과 동시에, 현재 빌드의 버전 정보를 포톤네트워크 객체에 기억시킴
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr"; //서버 강제 설정
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true; //서버 강제화
        PhotonNetwork.NickName = "456"; //닉네임도 설정할수 있다. (인풋필드에서 PlayerPrefs를 이용해서 간단하게 할수도있다. OnEndEdit이벤트 이용해서 이름 설정 가능)

        //버튼 연결
        goLobbyButton.onClick.AddListener(OnGoToLobby);
    }
    public void Start()
    {
        SoundManager.Instance.PlayBackgroundMusic(_titelSound);
    }
    private void OnDestroy()
    {
        goLobbyButton.onClick.RemoveAllListeners();
    }


    public void OnGoToLobby()
    {
        //서버에 접속부터 해준다
        PhotonNetwork.ConnectUsingSettings();
        //로비에 가야한다면?
        //SceneManager.LoadScene(1);
    }
    public override void OnConnectedToMaster()
    {
        //플레이어가 뭔가 다같이 이동할 필요가 없으므로 그냥 씬만 이동시켜준다
        Debug.Log("접속 성공!!");
        SceneManager.LoadScene(1);
    }

}
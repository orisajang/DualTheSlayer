using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviourPunCallbacks
{
    //현재 플레이어 ID
    public int CurrentPlayerId { get; private set; }
    private int _currentPlayerIndex;

    [SerializeField] private Button _endTurnButton;

    private List<int> _plyIdArray = new List<int>();
    private void Start()
    {
        //초기에 플레이어 프로퍼티를 확인하면서 플레이어정보를 읽음
        foreach(Player ply in PhotonNetwork.PlayerList)
        {
            bool isPlayer = (bool)ply.CustomProperties[NetworkEventManager.IsPlayer];
            //int plyIndex = (int)ply.CustomProperties[NetworkEventManager.SeatIndex];
            if(isPlayer)
            {
                _plyIdArray.Add(ply.ActorNumber);
            }
        }

        //초기 플레이어 랜덤으로 선택
        _currentPlayerIndex = Random.Range(0, 2);
        CurrentPlayerId = _plyIdArray[_currentPlayerIndex];

        //초기 플레이어 턴 시작
        InitPlayerTurnSetting();
    }

    

    private void InitPlayerTurnSetting()
    {
        foreach (Player ply in PhotonNetwork.PlayerList)
        {
            if (ply.ActorNumber == CurrentPlayerId)
            {
                //현재 플레이어의 ID 설정
                //SetPlayerID(ply.ActorNumber);
                if (ply.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    _endTurnButton.gameObject.SetActive(true);
                    break;
                }
                _endTurnButton.gameObject.SetActive(false);
            }
        }
    }

    public void OnClickEndTurnButton()
    {
        photonView.RPC(nameof(EndTurnRPC), RpcTarget.AllBuffered);
    }
    [PunRPC]
    private void EndTurnRPC()
    {
        _currentPlayerIndex++;
        //계속 index만 더하면서 턴을 번갈아서 사용하기 위해서 나누기 연산 진행
        int index = _currentPlayerIndex % _plyIdArray.Count;
        int currentPlayerId = _plyIdArray[index];
        SetPlayerID(currentPlayerId);

        //다음 플레이어턴 시작
        InitPlayerTurnSetting();
    }

    //현재 플레이 가능한 플레이어ID를 설정
    public void SetPlayerID(int id)
    {
        CurrentPlayerId = id;
    }

    //자신의 턴인지 확인하는 메서드
    public bool IsMyTurn(int id)
    {
        if (CurrentPlayerId == id) return true;
        else return false;
    }

    private void OnEnable()
    {
        //자신이 활성화되면 동적으로 GameManager에 자신 등록
        GameManager.Instance.SetTurnManager(this);
        //버튼 이벤트
        _endTurnButton.onClick.AddListener(OnClickEndTurnButton);
    }
    private void OnDisable()
    {
        GameManager.Instance.DeleteTurnManager(this);
        _endTurnButton.onClick.RemoveAllListeners();
    }

}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomTestView : MonoBehaviour
{
    [SerializeField] List<GameObject> playerSeatUI;
    [SerializeField] List<GameObject> spectorSeatUI;

    [SerializeField] Button playerTeamButton;
    [SerializeField] Button spectorTeamButton;

    RoomTest roomTest;

    public RoomTest RoomTest => roomTest;

    private void Awake()
    {
        roomTest = new RoomTest();

        //roomTest.OnSeatChanged += PhotonNetwokUpdate;
        playerTeamButton.onClick.AddListener(roomTest.MovePlayerTeam);
        spectorTeamButton.onClick.AddListener(roomTest.MoveSpectorTeam);

    }

    public void UIUpdateByData()
    {
        //플레이어 룸 정보 설정
        for(int i=0; i< playerSeatUI.Count; i++)
        {
            //매번 GetComponet로 하는것 개선하려면 딕셔너리로 캐싱해두고 꺼내쓰자 (지금은 그냥 쓰기)
            PlayerInfoUIPrefab prefab = playerSeatUI[i].GetComponent<PlayerInfoUIPrefab>();

            string playerName = roomTest.PlayerSeats[i].playerId;
            string teamName = RoomTest.PLAYER_SEATS;

            prefab.SetPlayerInfoUI(playerName, teamName);
        }
        //관전자 룸 정보 설정
        for(int i=0; i< spectorSeatUI.Count; i++)
        {
            PlayerInfoUIPrefab prefab = spectorSeatUI[i].GetComponent<PlayerInfoUIPrefab>();
            string playerName = roomTest.SpectorSeats[i].playerId;
            string teamName = RoomTest.SPECTOR_SEATS;
            prefab.SetPlayerInfoUI(playerName, teamName);
        }

    }
}

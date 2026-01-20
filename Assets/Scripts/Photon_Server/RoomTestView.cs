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



    public void UIUpdate()
    {
        IReadOnlyList<Seats> plySeatInfo = roomTest.PlayerSeats;

        for(int index = 0; index< roomTest.PlayerSeats.Count; index++)
        {
            // playerSeatUI[index].;
            PlayerInfoUIPrefab prefab = playerSeatUI[index].GetComponent<PlayerInfoUIPrefab>();
            prefab.SetPlayerInfoUI(roomTest.PlayerSeats[index].playerId, "Player");
        }

        for(int index = 0; index< roomTest.SpectorSeats.Count; index++)
        {
            PlayerInfoUIPrefab prefab = spectorSeatUI[index].GetComponent<PlayerInfoUIPrefab>();
            prefab.SetPlayerInfoUI(roomTest.SpectorSeats[index].playerId, "Spector");
        }

    }
}

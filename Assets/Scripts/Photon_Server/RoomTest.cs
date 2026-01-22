using Firebase.Auth;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum eSeatType
{
    Open,Player,Spector
}
public class Seats
{
    public string playerId;
    public int seatIndex;
    public eSeatType seatType;

    public void SetSeatInfo(string id,int index, eSeatType type)
    {
        playerId = id;
        seatIndex = index;
        seatType = type;
    }
    public void InitInfo()
    {
        playerId = "";
        seatType = eSeatType.Open;
    }
}

public class RoomTest
{
    List<Seats> playerSeats = new List<Seats>();
    List<Seats> spectorSeats = new List<Seats>();

    //설정된 플레이어 ID 체크
    Dictionary<string, Seats> seatDic = new Dictionary<string, Seats>();

    public IReadOnlyList<Seats> PlayerSeats => playerSeats;
    public IReadOnlyList<Seats> SpectorSeats => spectorSeats;

    public Action<string> OnSeatChanged;
    
    public RoomTest()
    {
        Init();

        LoadSeatRoomData();
    }

    private void Init()
    {
        for(int i=0; i< 2; i++)
        {
            playerSeats.Add(new Seats());
        }
        for(int i=0; i<6; i++)
        {
            spectorSeats.Add(new Seats());
        }
    }

    public void MovePlayerTeam()
    {
        //string myId = PhotonNetwork.LocalPlayer.UserId;
        string dbID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        int seatIndex = FindEmptySeat(playerSeats);
        if (seatIndex == -1) return;
        else
        {
            Debug.Log(seatIndex + "번째 플레이어 자리에 채워질것입니다");
            //기존에 앉아있었다면 자리 지워줌(딕셔너리)
            if(seatDic.ContainsKey(dbID))
            {
                seatDic[dbID].InitInfo();
            }
            //자리 설정
            playerSeats[seatIndex].SetSeatInfo(dbID, seatIndex, eSeatType.Player);
            seatDic[dbID] = playerSeats[seatIndex];

            SaveSeatRoomData();

            OnSeatChanged?.Invoke(eSeatType.Player.ToString());
            //포톤 네트워크에 정보를 보냄

            
        }
        
    }
    public void MoveSpectorTeam()
    {
        string dbID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        int seatIndex = FindEmptySeat(spectorSeats);
        if (seatIndex == -1) return;
        else
        {
            Debug.Log(seatIndex + "번째 관전자 자리에 채워질것입니다");
            if (seatDic.ContainsKey(dbID))
            {
                seatDic[dbID].InitInfo();
            }
            spectorSeats[seatIndex].SetSeatInfo(dbID, seatIndex, eSeatType.Spector);
            seatDic[dbID] = spectorSeats[seatIndex];

            //포톤 네트워크에 정보를 보냄
            SaveSeatRoomData();
            OnSeatChanged?.Invoke(eSeatType.Spector.ToString());
        }
        
    }

    private int FindEmptySeat(List<Seats> seats)
    {
        int findIndex = -1;

        for (int index = 0; index < seats.Count; index++)
        {
            //내가 이미 존재하면 안함
            if (seats[index].playerId == FirebaseAuth.DefaultInstance.CurrentUser.UserId)
            {
                return -1;
            }
            if (seats[index].playerId == "" || seats[index].playerId == null)
            {
                //중간이 비어있을수 있으므로 다 찾아보기
                if(findIndex == -1)
                {
                    findIndex = index;
                }
            }
        }
        return findIndex;
    }

    //커스텀 프로퍼티를 설정하여 서버에 해당 정보를 저장하고, 그 정보를 불러와서 전체 사용자가 그 정보를 볼수있음
    public void SaveSeatRoomData()
    {
        Room room = PhotonNetwork.CurrentRoom;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        //키값과 Value값을 설정한다.
        //지금 내가 보내야할거. 대기실에 보여야할 Player의 이름들. 즉 string 배열을 보내야한다
        string[] playerName = new string[playerSeats.Count];
        string[] spectorName = new string[spectorSeats.Count];
        for(int i=0; i< playerSeats.Count; i++)
        {
            //null이 들어가면 에러난다. 그래서 예외처리함
            playerName[i] = playerSeats[i].playerId != null ? playerSeats[i].playerId : "";
        }
        for(int i=0; i< spectorSeats.Count; i++)
        {
            spectorName[i] = spectorSeats[i].playerId != null ? spectorSeats[i].playerId : "";
        }
        //해쉬에 넣어준다. (키와 값을 설정하기 위해)
        hash.Add(NetworkEventManager.PLAYER_SEATS, playerName);
        hash.Add(NetworkEventManager.SPECTOR_SEATS, spectorName);
        //방 설정 저장
        room.SetCustomProperties(hash);

    }
    
    public void LoadSeatRoomData()
    {
        //방 정보를 불러와서 업데이트 해줘야함
        if (PhotonNetwork.CurrentRoom == null) return;
        //커스텀 프로퍼티 정보들을 불러온다
        ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        if(hash.ContainsKey(NetworkEventManager.PLAYER_SEATS))
        {
            string[] playerNameArray = (string[])hash[NetworkEventManager.PLAYER_SEATS];
            for(int i=0; i< playerNameArray.Length; i++)
            {
                playerSeats[i].playerId = playerNameArray[i];
            }
        }
        if (hash.ContainsKey(NetworkEventManager.SPECTOR_SEATS))
        {
            string[] spectorPlayerName = (string[])hash[NetworkEventManager.SPECTOR_SEATS];
            for(int i=0; i< spectorPlayerName.Length; i++)
            {
                spectorSeats[i].playerId = spectorPlayerName[i];
            }
        }
    }
}

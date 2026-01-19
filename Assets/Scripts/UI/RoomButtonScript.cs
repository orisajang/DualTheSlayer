using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomButtonScript : MonoBehaviour
{
    [SerializeField] Button roomButton;
    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] TextMeshProUGUI playerCountText;

    public void Init(LobbyNetworkMgr networkMgr,string buttonName, string playerCount, string maxPlayer)
    {
        roomButton.onClick.RemoveAllListeners();
        roomButton.onClick.AddListener(() => EnterTheRoom(networkMgr));
        roomNameText.text = buttonName;
        playerCountText.text = $"{playerCount} / {maxPlayer}";
    }

    public void UpdateRoomInfo(string buttonName, string playerCount, string maxPlayer)
    {
        roomNameText.text = buttonName;
        playerCountText.text = $"{playerCount} / {maxPlayer}";
    }

    public void EnterTheRoom(LobbyNetworkMgr networkManager)
    {
        networkManager.JoinRoomByName(roomNameText.text);
    }
}

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomInfoScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomNameText;

    private void Start()
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomNameText.text = room.Name;
    }
}

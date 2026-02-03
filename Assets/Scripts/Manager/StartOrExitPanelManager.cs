using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartOrExitPanelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Button _startButton;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient) { _startButton.gameObject.SetActive(true); }
        else { _startButton.gameObject.SetActive(false); }
    }
}

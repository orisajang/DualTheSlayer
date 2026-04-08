using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] GameObject _panel;
    [SerializeField] Button _closeButton;
    [SerializeField] Button _createButton;
    [SerializeField] TMP_InputField field;

    public void Init(UILobbyManager uimanger, LobbyNetworkMgr lobbyNetworkMgr)
    {
        _closeButton.onClick.RemoveAllListeners();
        _createButton.onClick.RemoveAllListeners();

        _closeButton.onClick.AddListener(() => uimanger.Close(_panel));
        _createButton.onClick.AddListener(() => lobbyNetworkMgr.CreateRoom(field.text));
        //_createButton.onClick.AddListener(() => lobbyNetworkMgr.);
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveAllListeners();
        _createButton.onClick.RemoveAllListeners();
    }

}

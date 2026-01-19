using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyManager : Singleton<UILobbyManager>
{
    LobbyNetworkMgr lobbyMgr;

    [SerializeField] Button _createNewRoomButton;
    [SerializeField] GameObject _createNewRoomPanel;
    [SerializeField] CreateRoomUI _createroomUI;

    protected override void Awake()
    {
        isDestroyOnLoad = false;
        base.Awake();

        _createNewRoomButton.onClick.AddListener(ActiveCreateNewRoomWindow);
    }
    private void OnDestroy()
    {
        _createNewRoomButton.onClick.RemoveAllListeners();
    }
    public void ActiveCreateNewRoomWindow()
    {
        _createNewRoomPanel.SetActive(true);
        _createroomUI.Init(this, lobbyMgr);
    }

    public void SetLobbyManager(LobbyNetworkMgr manager)
    {
        lobbyMgr = manager;
    }
    public void DeleteLobbyManager(LobbyNetworkMgr manager)
    {
        lobbyMgr = null;
    }


    //비활성화 메서드
    public void Close(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}

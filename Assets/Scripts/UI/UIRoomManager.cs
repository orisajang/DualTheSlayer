using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomManager : Singleton<UIRoomManager>
{
    RoomNetworkMgr roomNetworkMgr;

    protected override void Awake()
    {
        isDestroyOnLoad = false;
        base.Awake();
    }
    public void SetRoomNetworkManager(RoomNetworkMgr manager)
    {
        roomNetworkMgr = manager;
    }
    public void RemoveRoomNetworkManager(RoomNetworkMgr manager)
    {
        roomNetworkMgr = null;
    }


}

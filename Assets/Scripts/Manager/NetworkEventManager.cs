using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkEventManager : MonoBehaviourPunCallbacks
{
    //방을 떠나고 로비를 입장할때 등 씬 이동 시 계속 살아있지않으면 이벤트를 받을 수없는 상황에 사용하는 매니저
    public static NetworkEventManager Instance { get; private set; }

    

    private void Awake()
    {
        // 싱글톤이랑 MonoBehaviourPunCallbacks 랑 둘다 같이 상속받을수없어서 그냥 여기다가 싱글톤 코드 넣음
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; //이게 없으면 자식의 Awake가 끝까지 실행됨
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }
    public override void OnLeftLobby()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("완료");
    }
}

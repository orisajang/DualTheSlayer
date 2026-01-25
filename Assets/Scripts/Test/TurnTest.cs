using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTest : MonoBehaviour
{
    List<int> plyIdArray = new List<int>();
    [SerializeField] Button testButton;
    //네트워크에서 플레이어 2명이 턴을 주고받으면서 로그를 찍어보자
    private void Start()
    {
        //플레이어 전체를 돌면서 관전자가 아닌 실제 플레이어를 가져온다
        foreach(Player ply in PhotonNetwork.PlayerList)
        {
            //플레이어 프로퍼티를 불러와서 타입 확인
            bool isPlayer = (bool)ply.CustomProperties[NetworkEventManager.IsPlayer];
            if (isPlayer)
            {
                plyIdArray.Add(ply.ActorNumber);
            }
        }
        //코루틴으로 돌리자
        StartCoroutine(StrtTurnAround());
    }

    IEnumerator StrtTurnAround()
    {
        int index = 0;
        while(true)
        {
            int currentPos = plyIdArray[index % plyIdArray.Count];
            bool isMyTurn = false;


            foreach (Player ply in PhotonNetwork.PlayerList)
            {
                if (ply.ActorNumber == currentPos)
                {
                    Debug.Log($"{ply.ActorNumber}의 차례입니다");
                    GameManager.Instance.turnManager.SetPlayerID(ply.ActorNumber.ToString());
                    //만약 자신 턴이라면
                    if (ply.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        isMyTurn = true;
                    }
                    break;
                }
            }
            testButton.gameObject.SetActive(isMyTurn);
            index++;
            yield return new WaitForSeconds(3.0f);
        }

        
    }
}

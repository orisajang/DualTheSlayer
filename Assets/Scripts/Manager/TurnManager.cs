using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //현재 플레이어 ID
    public string CurrentPlayerId { get; private set; }

    //현재 플레이 가능한 플레이어ID를 설정
    public void SetPlayerID(string id)
    {
        CurrentPlayerId = id;
    }

    //자신의 턴인지 확인하는 메서드
    public bool IsMyTurn(string id)
    {
        if (CurrentPlayerId == id) return true;
        else return false;
    }

    private void OnEnable()
    {
        GameManager.Instance.SetTurnManager(this);
    }
    private void OnDisable()
    {
        GameManager.Instance.DeleteTurnManager(this);
    }

}

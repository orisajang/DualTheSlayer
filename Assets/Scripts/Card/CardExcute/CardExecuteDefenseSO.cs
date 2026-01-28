using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/CardExecute/Defense", fileName = "CardExecuteDefenseData")]
public class CardExecuteDefenseSO : CardExecuteSO
{
    public override bool CanExecute(CardTargetInfoClass cardTargetInfoClass)
    {
        return true; //카드 사용조건없음. 바로 True보냄
    }

    public override void Execute(CardTargetInfoClass info)
    {
        Debug.Log("Defense");
        //info.UseCardPlayer.AddPlayerShield()
        info.UseCardPlayer.AddPlayerShield(amount, info.cardInstanceData.Cost);
        //플레이어의 손패에서 제거
        info.UseCardPlayer.RemovePlayerHand(info.cardInstanceData);
    }
}

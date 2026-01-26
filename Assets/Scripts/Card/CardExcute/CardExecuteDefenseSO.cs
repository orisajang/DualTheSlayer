using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/CardExecute/Defense", fileName = "CardExecuteDefenseData")]
public class CardExecuteDefenseSO : CardExecuteSO
{
    public override bool Execute(CardTargetInfoClass info)
    {
        Debug.Log("Defense");
        //;
        //info.UseCardPlayer.AddPlayerShield()
        info.UseCardPlayer.AddPlayerShield(amount);
        return true;
    }
}

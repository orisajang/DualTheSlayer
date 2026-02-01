using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Card/CardExecute/Defense", fileName = "CardExecuteDefenseData")]
public class CardExecuteDefenseSO : CardExecuteSO
{
    private string _description;
    public override string description => _description;

    public override bool CanExecute(CardTargetInfoClass cardTargetInfoClass)
    {
        return true; //카드 사용조건없음. 바로 True보냄
    }

    public override string CardInitDescription()
    {
        return $"방어도를 {amount}만큼 얻습니다";
    }

    public override void Execute(CardTargetInfoClass info)
    {
        //공통 기능 사용(카드 풀에 반환)
        base.Execute(info);
        Debug.Log("Defense");
        info.UseCardPlayer.AddPlayerShield(amount, info.cardInstanceData.Cost);
        
    }
}

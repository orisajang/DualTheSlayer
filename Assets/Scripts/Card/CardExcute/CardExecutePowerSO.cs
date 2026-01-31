using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Card/CardExecute/Power", fileName = "CardExecutePowerData")]
public class CardExecutePowerSO : CardExecuteSO
{
    public override bool CanExecute(CardTargetInfoClass cardTargetInfoClass)
    {
        return true; //조건 없음
    }
    public override void Execute(CardTargetInfoClass cardTargetInfoClass)
    {
        base.Execute(cardTargetInfoClass);
        //자기자신에게 힘 버프를 준다
        cardTargetInfoClass.UseCardPlayer.PlayerCondition.AddConditionStatus(eConditionType.Power, amount, 1, cardTargetInfoClass.UseCardPlayer.photonView.Owner.ActorNumber);
    }
}

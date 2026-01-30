using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Card/CardExecute/Heal", fileName = "CardExecuteHealData")]
public class CardExecuteHealSO : CardExecuteSO
{
    //힐이 몇턴동안 지속되는지 여부
    [SerializeField] private int duration;
    public override bool CanExecute(CardTargetInfoClass cardTargetInfoClass)
    {
        return true; //예외처리로 카드실행 취소되어야하는경우가 없으므로 무조건 true를 보내준다
    }

    public override void Execute(CardTargetInfoClass cardTargetInfoClass)
    {
        //공통 기능 사용(카드 풀에 반환)
        base.Execute(cardTargetInfoClass);
        //힐 기능 사용
        cardTargetInfoClass.UseCardPlayer.AddDotHealStatus(amount, duration,cardTargetInfoClass.cardInstanceData.Cost);
    }
}

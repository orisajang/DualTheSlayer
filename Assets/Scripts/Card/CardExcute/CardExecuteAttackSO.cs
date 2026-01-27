using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/CardExecute/Attack", fileName = "CardExecuteAttackData")]
public class CardExecuteAttackSO : CardExecuteSO
{
    public override bool Execute(CardTargetInfoClass cardTargetInfoClass)
    {
        Debug.Log("공격 명령");
        //자신의 힘(Power)수치를 더해서 공격력에 추가시켜주면 될듯? 일단은 카드수치만 적용

        if(cardTargetInfoClass.TargetPlayer == GameManager.Instance.playerManager)
        {
            Debug.Log("자기 자신을 공격할수 없습니다");
            return false;
        }
        //상대에게 데미지 처리
        cardTargetInfoClass.TargetPlayer.TakeDamage(amount);
        //자신의 행동력 감소
        cardTargetInfoClass.UseCardPlayer.DecreaseEnergy(cardTargetInfoClass.CardCost);
        return true;
    }
}

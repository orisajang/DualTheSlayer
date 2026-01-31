using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Card/CardExecute/Attack", fileName = "CardExecuteAttackData")]
public class CardExecuteAttackSO : CardExecuteSO
{
    //Execute가 리스트로 바뀌어서 실행전 Execute들이 하나도 빠짐없이 실행가능한지 체크해야해서 추가
    public override bool CanExecute(CardTargetInfoClass cardTargetInfoClass)
    {
        if (cardTargetInfoClass.TargetPlayer == GameManager.Instance.playerManager)
        {
            Debug.Log("자기 자신을 공격할수 없습니다");
            return false;
        }
        return true;
    }

    public override void Execute(CardTargetInfoClass cardTargetInfoClass)
    {
        //공통 기능 사용(카드 풀에 반환)
        base.Execute(cardTargetInfoClass);
        Debug.Log("공격 명령");
        //자신의 힘(Power)수치를 더해서 공격력에 추가시켜주면 될듯? 일단은 카드수치만 적용

        //상대에게 데미지 처리
        int addValue = cardTargetInfoClass.UseCardPlayer.PlayerCondition.GetPlayerPower();
        Debug.Log($"힘으로 인해 {addValue} 숫자가 더해졌습니다");
        cardTargetInfoClass.TargetPlayer.TakeDamage(amount + addValue, cardTargetInfoClass.UseCardPlayer.photonView.Owner.ActorNumber);
    }
}

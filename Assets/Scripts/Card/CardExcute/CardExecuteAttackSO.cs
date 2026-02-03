using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Card/CardExecute/Attack", fileName = "CardExecuteAttackData")]
public class CardExecuteAttackSO : CardExecuteSO
{
    private string _description;
    public override string description => _description;

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

    public override string CardInitDescription()
    {
        return $"데미지를 {amount}만큼 줍니다";
    }
    public override string CardSetDescription(int addPowerValue)
    {
        //힘이 0으로 들어올경우 증가하는게 없으므로 기본 카드 설명값을 return으로 보내준다
        if (addPowerValue == 0) { return base.CardSetDescription(addPowerValue); }
        else { return $"데미지를 {amount + addPowerValue}만큼 줍니다 (<color=green>+{amount}+{addPowerValue}</color>) "; }       
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
        //실제 데미지 처리하는 메서드로 이동
        cardTargetInfoClass.TargetPlayer.TakeDamage(amount + addValue, cardTargetInfoClass.UseCardPlayer.photonView.Owner.ActorNumber);
        //소리 재생
        cardTargetInfoClass.UseCardPlayer.PlayPlayerAttackSound();
    }
}

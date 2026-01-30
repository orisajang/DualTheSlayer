using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적에게 출혈 상태이상을 부여하는 SO 행동
[CreateAssetMenu(menuName = "SO/Card/CardExecute/Bleeding", fileName = "CardExecuteBleedingData")]
public class CardExecuteBleedingSO : CardExecuteSO
{
    [Header("출혈횟수")]
    [SerializeField] private int bleedingCount;
    public override bool CanExecute(CardTargetInfoClass cardTargetInfoClass)
    {
        //카드 사용가능한지 조건체크
        if(cardTargetInfoClass.TargetPlayer == cardTargetInfoClass.UseCardPlayer)
        {
            Debug.Log($"자기자신이 카드사용 대상이 될수없습니다");
            return false;
        }
        return true;
    }
    public override void Execute(CardTargetInfoClass cardTargetInfoClass)
    {
        //공통 기능 사용(카드 풀에 반환)
        base.Execute(cardTargetInfoClass);
        //피격자 출혈수치 증가
        cardTargetInfoClass.TargetPlayer.AddBleedingStatus(amount, bleedingCount, cardTargetInfoClass.cardInstanceData.Cost, cardTargetInfoClass.UseCardPlayer.photonView.Owner.ActorNumber);
        //카드 사용한사람 행동력 감소
        //cardTargetInfoClass.UseCardPlayer.DecreaseEnergy(cardTargetInfoClass.cardInstanceData.Cost);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardExecuteSO : ScriptableObject
{
    //카드를 실행할때 얼마만큼의 값이 적용되는지 (데미지,쉴드,힐량 등등)
    [SerializeField] protected int amount;
    public abstract string description { get; }
    //카드를 실행시키면 해야할 동작들을 정의 (virtual로 해둬서 기본동작을 정의했음. 사용할때 override로 동작들도 추가해야함)
    public virtual void Execute(CardTargetInfoClass cardTargetInfoClass)
    {
        //플레이어의 손패에서 제거
        cardTargetInfoClass.UseCardPlayer.RemovePlayerHand(cardTargetInfoClass.cardInstanceData);
    }
    //이 카드를 실행할 수 있는 조건인지 체크
    public abstract bool CanExecute(CardTargetInfoClass cardTargetInfoClass); //이거 추가?
    //기본으로 이 카드가 가지고있는 카드 설명
    public abstract string CardInitDescription();
    //만약에 힘, 보호 이런 것들로 인해 변경된 텍스트의 정보를 보내야하면 자식에서 override해서 쓸수있도록 정해둠
    public virtual string CardSetDescription(int addPowerValue)
    {
        return CardInitDescription();
    }
}

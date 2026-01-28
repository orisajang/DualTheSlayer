using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardExecuteSO : ScriptableObject
{
    //카드를 실행할때 얼마만큼의 값이 적용되는지 (데미지,쉴드,힐량 등등)
    public int amount;
    public abstract void Execute(CardTargetInfoClass cardTargetInfoClass); //이걸 리스트로?? ?

    public abstract bool CanExecute(CardTargetInfoClass cardTargetInfoClass); //이거 추가?
}

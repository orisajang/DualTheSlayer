using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardExecuteSO : ScriptableObject
{
    //카드를 실행할때 어떤 것들이 있어야 하는지에 대해서
    public int amount;
    public abstract void Execute(CardTargetInfoClass cardView);
}

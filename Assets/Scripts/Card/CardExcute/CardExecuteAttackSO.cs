using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/CardExecute/Attack", fileName = "CardExecuteAttackData")]
public class CardExecuteAttackSO : CardExecuteSO
{
    public override void Execute(CardView cardView)
    {
        Debug.Log("공격 명령");
        CardSpawner.Instance.ReturnCardToPool(cardView);
    }
}

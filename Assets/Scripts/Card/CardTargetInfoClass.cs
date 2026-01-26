using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTargetInfoClass
{
    //카드 사용자와 타겟의 정보를 담은 클래스 
    public PlayerManager UseCardPlayer { get; private set; }
    public PlayerManager TargetPlayer { get; private set; }
    public int CardCost;
    public CardTargetInfoClass(PlayerManager user, PlayerManager target, int cost)
    {
        UseCardPlayer = user;
        TargetPlayer = target;
        CardCost = cost;
    }
}

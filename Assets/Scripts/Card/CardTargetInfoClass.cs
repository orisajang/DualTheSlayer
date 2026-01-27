using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTargetInfoClass
{
    //카드 사용자와 타겟의 정보를 담은 클래스 
    public PlayerManager UseCardPlayer { get; private set; }
    public PlayerManager TargetPlayer { get; private set; }
    //아래에 있는 데이터 CardTargetInfoClass에서 바꾸면 안됨.
    public CardInstance cardInstanceData { get; private set; }
    public CardTargetInfoClass(PlayerManager user, PlayerManager target, CardInstance instance)
    {
        UseCardPlayer = user;
        TargetPlayer = target;
        cardInstanceData = instance;
    }
}

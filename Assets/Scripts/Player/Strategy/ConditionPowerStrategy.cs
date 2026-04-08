using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionPowerStrategy : ConditionStrategy
{
    public override void SetConditionUIData(PlayerConditionSpawner playerConditionSpawner, Dictionary<eConditionType, PlayerConditionUI> playerConditionDic,
        int amount, int duration, eConditionType buffType,bool isStackAble)
    {
        if (!playerConditionDic.ContainsKey(buffType))
        {
            //플레이어 상태이상UI를 새로 만들어줌
            PlayerConditionUI playerConditionUI = playerConditionSpawner.GetPlayerBuffUIByPool();
            playerConditionUI.SetConditionInfo(amount, duration, buffType, isStackAble);
            playerConditionDic.Add(buffType, playerConditionUI);
        }
        else
        {
            //버프횟수 추가
            playerConditionDic[buffType].AddConditionInfo(amount, duration, buffType, isStackAble);
        }
    }
}

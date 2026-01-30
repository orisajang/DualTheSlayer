using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionBleedingStrategy : ConditionStrategy
{
    public override void SetConditionUIData(PlayerConditionSpawner playerConditionSpawner, Dictionary<eConditionType, PlayerConditionUI> playerConditionDic, int amount, int duration, eConditionType buffType)
    {
        if(!playerConditionDic.ContainsKey(buffType))
        {
            //새로 만들어줌
            PlayerConditionUI playerConditionUI = playerConditionSpawner.GetPlayerBuffUIByPool();
            playerConditionUI.SetConditionInfo(amount, duration, buffType);
            playerConditionDic.Add(buffType, playerConditionUI);
        }
        else
        {
            //버프횟수 추가
            playerConditionDic[buffType].AddConditionInfo(amount, duration, buffType);
        }
        
    }
}

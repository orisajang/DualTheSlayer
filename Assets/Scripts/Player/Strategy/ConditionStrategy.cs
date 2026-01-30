using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionStrategy
{

    public abstract void SetConditionUIData(PlayerConditionSpawner playerConditionSpawner, Dictionary<eConditionType, PlayerConditionUI> playerConditionDic,int amount, int duration, eConditionType buffType);

}

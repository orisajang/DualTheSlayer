using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Player/PlayerCondition Data", fileName = "PlayerConditionData")]
public class PlayerConditionTypeSO : ScriptableObject
{
    //플레이어의 버프 종류에 대해 어떤것들이 있는지 설정하기 위한 SO
    //해당 버프가 어떤 이미지를 쓰고있는지 설정하기 위해서
    [SerializeField] private eConditionType conditionType;
    [SerializeField] private Sprite image;

    public eConditionType ConditionType => conditionType;
    public Sprite Image => image;

}

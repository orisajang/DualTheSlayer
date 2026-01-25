using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig
{
    //플레이어 설정할때 어떤것들이 들어가는지 담아둔 클래스
    public RectTransform cardSpawnPosition;
    public GameObject cardPrefab;
    public RectTransform useCardLine;
    public string id;
    public List<CardSOClass> deck;
    public PlayerLevelData levelData;

}

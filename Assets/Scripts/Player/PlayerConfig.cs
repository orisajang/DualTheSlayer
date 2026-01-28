using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig
{
    //플레이어 설정할때 어떤것들이 들어가는지 담아둔 클래스
    public RectTransform cardSpawnPosition;
    public GameObject cardPrefab;
    public RectTransform useCardLine;
    public int id;
    public List<CardSO> originDeck; //원본데이터라서 안의값 수정하면안됨
    public PlayerLevelData levelData;

}

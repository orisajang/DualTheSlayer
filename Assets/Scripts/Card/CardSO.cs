using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardType
{
    Attack, Defense, Special
}
public enum eTargetType
{
    Target, NotTarget
}

[CreateAssetMenu(menuName = "Card/Card Data", fileName = "CardData")]
public class CardSO : ScriptableObject
{
    //유니티에서 제공하는 ScriptableObject를 이용해서 Inspector에서 Stage정보를 설정할 수 있도록 설정
    [SerializeField] private string cardName;
    [SerializeField] private int cost; //카드 비용
    [SerializeField] private eCardType cardType; //카드 타입
    [SerializeField] private Sprite cardImage;//카드 이미지
    [SerializeField] private string description; //카드 설명
    [SerializeField] private eTargetType targetAble; //타게팅 가능한 스킬인지

    [SerializeField] CardExecuteSO executeSO;

    public string CardName => cardName;
    public int Cost => cost;
    public eCardType CardType => cardType;
    public Sprite CardImage => cardImage;
    public string Description => description;
    public eTargetType TargetAble => targetAble;
    public CardExecuteSO ExecuteSO => executeSO;
}

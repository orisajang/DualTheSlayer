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
//(SO를 만든뒤 Resource/CardData폴더에 넣어둬야함)
[CreateAssetMenu(menuName = "Card/Card Data", fileName = "CardData")]
public class CardSO : ScriptableObject
{
    //유니티에서 제공하는 ScriptableObject를 이용해서 Inspector에서 Stage정보를 설정할 수 있도록 설정
    [SerializeField] private string cardId; //카드 고유 ID
    [SerializeField] private string cardName; //카드 이름
    [SerializeField] private int cost; //카드 비용
    [SerializeField] private eCardType cardType; //카드 타입
    [SerializeField] private Sprite cardImage;//카드 이미지
    [SerializeField] private string description; //카드 설명
    [SerializeField] private eTargetType targetAble; //타게팅 가능한 스킬인지
    //실행되면 무엇을 해야하는지
    [SerializeField] List<CardExecuteSO> executeSO;


    //읽기전용 프로퍼티들
    public string CardId => cardId;
    public string CardName => cardName;
    public int Cost => cost;
    public eCardType CardType => cardType;
    public Sprite CardImage => cardImage;
    public string Description => description;
    public eTargetType TargetAble => targetAble;
    public List<CardExecuteSO> ExecuteSO => executeSO;
}

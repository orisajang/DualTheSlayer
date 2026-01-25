using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSOClass
{
    //CardSO의 데이터를 받아서 클래스로 만들어주기 위해 사용
    public string CardID { get; private set; }
    public string CardName { get; private set; } //카드 이름
    public int Cost { get; private set; } //카드 비용
    public eCardType CardType { get; private set; } //카드 타입
    public Sprite CardImage { get; private set; }//카드 이미지
    public string Description { get; private set; } //카드 설명
    public eTargetType TargetAble { get; private set; } //타게팅 가능한 스킬인지
                                                        //실행되면 무엇을 해야하는지
    public CardExecuteSO ExecuteSO { get; private set; }
    public CardSOClass(CardSO data)
    {
        CardID = data.CardId;
        CardName = data.CardName;
        Cost = data.Cost;
        CardType = data.CardType;
        CardImage = data.CardImage;
        Description = data.Description;
        TargetAble = data.TargetAble;
        ExecuteSO = data.ExecuteSO;
    }
}

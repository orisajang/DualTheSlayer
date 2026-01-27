using UnityEngine;

public class CardInstance
{
    //cardData는 실제 카드 데이터와 연관되어있으므로 값을 바꾸면 안됨
    private CardSOClass cardData;
    int currentCost;

    public CardSOClass CardData => cardData;

    public CardInstance(CardSOClass data)
    {
        cardData = data;
        currentCost = cardData.Cost;
    }
}
using UnityEngine;

public class CardInstance
{
    private CardSOClass cardData;
    int currentCost;

    public CardSOClass CardData => cardData;

    public CardInstance(CardSOClass data)
    {
        cardData = data;
        currentCost = cardData.Cost;
    }
}
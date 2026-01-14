using UnityEngine;

public class CardInstance
{
    private Card cardData;
    int currentCost;

    public Card CardData => cardData;

    public CardInstance(Card data)
    {
        cardData = data;
        currentCost = cardData.Cost;
    }
}
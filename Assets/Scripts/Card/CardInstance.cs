using UnityEngine;

public class CardInstance
{
    private CardSO cardData;
    int currentCost;

    public CardSO CardData => cardData;

    public CardInstance(CardSO data)
    {
        cardData = data;
        currentCost = cardData.Cost;
    }
}
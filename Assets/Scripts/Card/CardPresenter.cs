using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICardMVPView
{
    public void SetCardResource(Sprite sprite, string text);
}


public class CardPresenter
{
    CardModel cardModel;
    ICardMVPView cardView;
    public CardPresenter(CardModel model, ICardMVPView view)
    {
        cardModel = model;
        cardView = view;
    }
    public void UpdateCardUI()
    {
        cardView.SetCardResource(cardModel.CardInstance.CardData.CardImage, cardModel.CardInstance.CardData.Description);
    }
    public eTargetType GetInputType()
    {
        return cardModel.CardInstance.CardData.TargetAble;
    }

    public void ExecuteCard(Enemy enemy)
    {
        cardModel.CardInstance.CardData.ExecuteSO.Execute();
    }
}

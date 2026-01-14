using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICardMVPView
{
    public void SetCardData(CardInstance cardInstance);
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
}

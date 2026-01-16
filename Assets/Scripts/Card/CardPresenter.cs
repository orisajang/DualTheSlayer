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

    public void ExecuteCard(Enemy enemy, CardView view)
    {
        //적의 hp, 나의 공격력들을 받아서 BattleManager에 데미지 계산한후에 적용시키는 로직 필요
        cardModel.CardInstance.CardData.ExecuteSO.Execute(view);
        CardSpawner.Instance.ReturnCardToPool(view);
    }
    public void ExecuteCard(CardView view)
    {
        cardModel.CardInstance.CardData.ExecuteSO.Execute(view);
        CardSpawner.Instance.ReturnCardToPool(view);
    }
}

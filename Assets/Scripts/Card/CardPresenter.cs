using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICardMVPView
{
    public void UpdateCardUI();
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
    //카드 UI 갱신(아래의 플레이어 손패)
    public void UpdateCardForUI()
    {
        cardView.UpdateCardUI();
    }

    public eTargetType GetInputType()
    {
        return cardModel.CardInstance.TargetAble;
    }

    public void ExecuteCard(CardView view, CardTargetInfoClass cardTargetInfoClass)
    {
        //bool isSuccess = cardModel.CardInstance.ExecuteSO.Execute(cardTargetInfoClass);
        bool isSuccess = cardModel.CardInstance.ExecuteAll(cardTargetInfoClass);
        //타겟팅 카드의 경우 대상이 아니면 false보내고 카드실행 취소해야해서 bool판별해야함
        if(isSuccess)
        {
            //카드를 다 썼으면 그때 이제

            //RPC로 전체 사용자들한테 어떤 카드 썻는지 보여주는 표시
            view.UpdateCardInfoUI();
            //카드덱 오브젝트풀에 객체 반환
            CardSpawner.Instance.ReturnCardToPool(view);
        }
    }
    public CardInstance ReturnCardData()
    {
        return cardModel.CardInstance;
    }
}

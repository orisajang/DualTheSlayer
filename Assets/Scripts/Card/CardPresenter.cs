using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICardMVPView
{
    public void SetCardResource(Sprite sprite, string text, string cost);
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
    public void UpdateCardUI()
    {
        cardView.SetCardResource(cardModel.CardInstance.CardData.CardImage, cardModel.CardInstance.CardData.Description, cardModel.CardInstance.CardData.Cost.ToString());
    }

    public eTargetType GetInputType()
    {
        return cardModel.CardInstance.CardData.TargetAble;
    }

    public void ExecuteCard(Enemy enemy, CardView view, CardTargetInfoClass cardTargetInfoClass)
    {
        //적의 hp, 나의 공격력들을 받아서 BattleManager에 데미지 계산한후에 적용시키는 로직 필요
        cardModel.CardInstance.CardData.ExecuteSO.Execute(cardTargetInfoClass);
        //RPC로 전체 사용자들한테 어떤 카드 썻는지 보여주는 표시
        view.UpdateCardInfoUI();
        //카드덱 오브젝트풀에 객체 반환
        CardSpawner.Instance.ReturnCardToPool(view);
    }
    public void ExecuteCard(CardView view, CardTargetInfoClass cardTargetInfoClass)
    {
        bool isSuccess = cardModel.CardInstance.CardData.ExecuteSO.Execute(cardTargetInfoClass);
        //타겟팅 카드의 경우 대상이 아니면 false보내고 카드실행 취소해야해서 bool판별해야함
        if(isSuccess)
        {
            //RPC로 전체 사용자들한테 어떤 카드 썻는지 보여주는 표시
            view.UpdateCardInfoUI();
            //카드덱 오브젝트풀에 객체 반환
            CardSpawner.Instance.ReturnCardToPool(view);
        }
        else
        {
            Debug.Log("카드 타겟대상이 틀렸습니다 확인해주세요. 실행취소");
        }
    }
    public CardInstance ReturnCardData()
    {
        return cardModel.CardInstance;
    }
}

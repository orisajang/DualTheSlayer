using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour, ICardMVPView
{
    [SerializeField] Image cardImage;
    [SerializeField] TextMeshProUGUI cardDescription;

    //MVP관련 설정
    CardPresenter cardPresenter;

    public void Init(CardInstance instance)
    {
        //카드 모델은 생성만하고 Presenter에 담자.
        CardModel cardModel = new CardModel();
        cardModel.Init(instance);
        cardPresenter = new CardPresenter(cardModel, this);
        //카드 이미지, 설명을 갱신
        cardPresenter.UpdateCardUI();
    }

    //Presenter에서 받아온 정보로 UI 갱신
    public void SetCardResource(Sprite sprite, string text)
    {
        cardImage.sprite = sprite;
        cardDescription.text = text;
    }

}

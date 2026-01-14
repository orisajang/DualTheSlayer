using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour, ICardMVPView
{
    [SerializeField] Image cardImage;
    [SerializeField] TextMeshProUGUI cardDescription;
    CardInstance currentCard;

    public void SetCardData(CardInstance cardInstance)
    {
        //카드 설정
        currentCard = cardInstance;
        //카드 이미지, 설명항목 채우기
        cardImage.sprite = cardInstance.CardData.CardImage;
        cardDescription.text = cardInstance.CardData.Description;
    }
}

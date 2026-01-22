using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoPrefab : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _cardName;
    [SerializeField] TextMeshProUGUI _cost;


    CardDataTestSO cardData;


    public void SetCardData(CardDataTestSO data)
    {
        cardData = data;

        _image.sprite = cardData.Image;
        _cardName.text = cardData.CardName;
        _cost.text = cardData.Cost.ToString();

    }
}

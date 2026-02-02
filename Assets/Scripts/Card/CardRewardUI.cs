using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardRewardUI : MonoBehaviour, IPointerDownHandler
{
    //카드 UI에 정보를 표시하기위해
    [SerializeField] Image _cardImage;
    [SerializeField] TextMeshProUGUI _cardDescription;
    [SerializeField] TextMeshProUGUI _costText;

    //선택되었을때 해당 UI 주변에 붉은 테두리 표시를 위해서
    [SerializeField] Image _outLineImage;

    //해당 카드를 선택했을때 어떤 카드ID로 만들어야하는지 정보 미리 설정해두기 위해
    public string CardId { get; private set; }

    //이벤트를 보낸다 (자기자신이 클릭되었다는)
    public event Action<CardRewardUI> OnClicked;

    public void SetCardRewadUI(Sprite image, string description, string cost, string id)
    {
        _cardImage.sprite = image;
        _cardDescription.text = description;
        _costText.text = cost;
        CardId = id;
    }
    public void SetOutLineUI(bool isActive)
    {
        _outLineImage.gameObject.SetActive(isActive);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //마우스 클릭되었을때 해당 UI의 테두리를 활성화시킨다.
        //이벤트를 보내도될듯? 
        OnClicked?.Invoke(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDeckMoveScript : MonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,IDragHandler,IEndDragHandler
{
    //덱설정창에서 카드UI들이 움직이면서 특정 영역으로 이동하기 위한 스크립트
    Transform _originParent; //원래부모
    Canvas _canvas; //캔버스 위치
    ScrollRect _scrollRectParent;

    CanvasGroup _canvasgroup;
    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _scrollRectParent = GetComponentInParent<ScrollRect>();
        _canvasgroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (_scrollRectParent != null) _scrollRectParent.enabled = false;
        _canvasgroup.blocksRaycasts = false;

        _originParent = gameObject.transform.parent;
        gameObject.transform.SetParent(_canvas.transform);
        transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //드래그를 놓으면 DeckDragableArea에서 체크를해서 영역을 바꿈
        //if (_scrollRectParent != null) _scrollRectParent.enabled = true;

        if(gameObject.transform.parent == _canvas.transform)
        {
            gameObject.transform.SetParent(_originParent);
        }

        _canvasgroup.blocksRaycasts = true;

        //throw new System.NotImplementedException();
        //gameObject.transform.SetParent(_originParent);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //마우스 드래그해서 카드 이동 잘 되는지 확인해야함. 지금 좀 안되네
        
        //드래그하는 동안에는 빼준다
        
        //gameObject.transform.SetParent(_canvas.transform);
    }


}

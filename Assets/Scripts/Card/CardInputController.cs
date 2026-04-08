using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//한곳에서 입력을 판단하고, 어떤 동작을 하는지 판단하는 클래스
public class CardInputController : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //입력(CardHover와 타겟지정 화살표에 대한 동작 판단)
    [SerializeField] CardHover cardHover;
    [SerializeField] CardArrorwUI cardArrorwUI;

    //드래그했을때 카드가 이동되어야하는지 판별하는 코드
    [SerializeField] CardView cardView;
    eTargetType targetType;

    public void Init()
    {
        targetType = cardView.GetTargetType();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardHover.PointerEnter(eventData);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //내턴이 아니면 return
        //if (!IsDragAble()) return;
        cardHover.PointerExit(eventData);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDragAble()) return;
        switch (targetType)
        {
            case eTargetType.NotTarget:
                cardHover.BeginDrag(eventData);
                break;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        //내턴이 아니면 return
        if (!IsDragAble()) return;
        //타겟팅 스킬인경우 화살표 UI, 아니라면 카드 그냥 내는 UI
        switch (targetType)
        {
            case eTargetType.Target:
                cardArrorwUI.Drag(eventData);
                break;
            case eTargetType.NotTarget:
                cardHover.Drag(eventData);
                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //내턴이 아니면 return
        if (!IsDragAble()) return;
        //타겟팅 스킬인경우 화살표 UI, 아니라면 카드 그냥 내는 UI
        switch (targetType)
        {
            case eTargetType.Target:
                cardArrorwUI.EndDrag(eventData);
                break;
            case eTargetType.NotTarget:
                cardHover.EndDrag(eventData);
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //내턴이 아니면 return
        if (!IsDragAble()) return;
        //타겟팅 스킬인경우 화살표 UI, 아니라면 카드 그냥 내는 UI
        switch (targetType)
        {
            case eTargetType.Target:
                cardArrorwUI.PointerDown(eventData);
                break;
            case eTargetType.NotTarget:
                cardHover.PointerDown(eventData);
                break;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //내턴이 아니면 return
        if (!IsDragAble()) return;
        //타겟팅 스킬인경우 화살표 UI, 아니라면 카드 그냥 내는 UI
        switch (targetType)
        {
            case eTargetType.Target:
                cardArrorwUI.PointerUp(eventData);
                break;
            case eTargetType.NotTarget:
                cardHover.PointerUp(eventData);
                break;
        }
    }
    private bool IsDragAble()
    {
        //현재 플레이어의 행동력이 카드의 코스트보다 많고 && 플레이어 자신의 턴일때만 카드 드래그 가능
        bool isDragable = cardView.IsUseableCard() && GameManager.Instance.turnManager.IsMyTurn(GameManager.Instance.playerManager.PlayerID);
        //Debug.Log($"카드드래깅 가능여부: {isDragable}");
        return isDragable;
    }
}

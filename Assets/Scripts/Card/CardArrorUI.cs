using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardArrorUI : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    //카드를 클릭하면 화살표UI가 나와서 타게팅 가능하도록
    UITargetArrow arrow;
    RectTransform rect;

    //카드 데이터를 얻어오기위해서
    [SerializeField] CardView cardView;
    eTargetType targetType;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        arrow = GameManager.Instance.targetingManager.Arrow;
    }
    public void Init()
    {
        targetType = cardView.GetTargetType();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //타겟팅 스킬이 아니면 return
        if (targetType != eTargetType.Target) return;
        //자신의 턴이 아니면 return
        if (!IsDragAble()) return;
        arrow.Show(rect.position, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //타겟팅 스킬이 아니면 return
        if (targetType != eTargetType.Target) return;
        //자신의 턴이 아니면 return
        if (!IsDragAble()) return;
        arrow.Show(rect.position, eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //타겟팅 스킬이 아니면 return
        if (targetType != eTargetType.Target) return;
        //자신의 턴이 아니면 return
        if (!IsDragAble()) return;
        arrow.Hide();
    }
    private bool IsDragAble()
    {
        //현재 자신의 ID로 자신의 턴인지 확인한다
        return GameManager.Instance.turnManager.IsMyTurn(GameManager.Instance.playerManager.PlayerID);
    }
}


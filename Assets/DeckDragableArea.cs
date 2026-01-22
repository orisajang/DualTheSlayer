using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckDragableArea : MonoBehaviour,
    IDropHandler,
    IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] GameObject contentArea;
    public void OnDrop(PointerEventData eventData)
    {
        CardDeckMoveScript prefab = eventData.pointerDrag?.GetComponent<CardDeckMoveScript>();
        if (prefab == null) return;
        prefab.transform.SetParent(contentArea.transform);

        //LayoutRebuilder.ForceRebuildLayoutImmediate(
        //    contentArea.GetComponent<RectTransform>()
        //    );

        //테스트하기
        //prefab.transform.localPosition = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
    }
}

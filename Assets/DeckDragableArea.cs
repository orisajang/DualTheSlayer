using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum eDropzoneType
{
    deck, inventory
}

public class DeckDragableArea : MonoBehaviour,
    IDropHandler,
    IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] GameObject contentArea;
    [SerializeField] eDropzoneType dropzoneType;
    public eDropzoneType DropzoneType => dropzoneType;
    public void OnDrop(PointerEventData eventData)
    {
        CardDeckMoveScript prefab = eventData.pointerDrag?.GetComponent<CardDeckMoveScript>();
        if (prefab == null) return;

        //이제 부모가 서로 다른지 확인해서 경우2가지로 해결해야함
        DeckDragableArea parent = prefab.OriginParent.GetComponentInParent<DeckDragableArea>();
        Debug.Log($"부모는: {parent.name}");
        if (gameObject.transform.parent == parent) return;
        //이제 어디에서 바꿨는지 확인해서 처리
        if(parent.DropzoneType == eDropzoneType.deck && DropzoneType == eDropzoneType.inventory)
        {
            //덱 -> 인벤토리로 이동했으므로
            CardInfoPrefab deckData = prefab.GetComponent<CardInfoPrefab>();
            deckData.transform.SetParent(prefab.OriginParent);
            DeckPoolSpawner.Instance.ReturnDeckCardToPool(deckData);
            CardInfoPrefab invenCard = DeckPoolSpawner.Instance.GetInventoryCardByPool();
            invenCard.SetCardData(deckData.cardData);
        }
        else if(parent.DropzoneType == eDropzoneType.inventory && DropzoneType == eDropzoneType.deck)
        {
            //인벤토리 -> 덱
            CardInfoPrefab invenData = prefab.GetComponent<CardInfoPrefab>();
            invenData.transform.SetParent(prefab.OriginParent);
            DeckPoolSpawner.Instance.ReturnInvenCardToPool(invenData);
            CardInfoPrefab deckCard = DeckPoolSpawner.Instance.GetDeckCardByPool();
            deckCard.SetCardData(invenData.cardData);
        }
        
        //prefab.transform.SetParent(contentArea.transform);
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

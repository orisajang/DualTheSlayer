using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //선택한 카드가 뭔지 보이게 하기위해 마우스를 올리거나 클릭했을때 카드가 커져보이게 만드는 기능
    //PC 마우스클릭(호버링)에 대응하기위해서 IPointerEnterHandler, 나갔을때 IPointerExitHandler
    //모바일은 클릭하면 커지도록 IPointerDownHandler, 클릭해제하면 IPointerUpHandler

    private Vector3 originalScale;

    private RectTransform rect;
    Canvas canvas;
    Transform originalParent;
    Vector2 originalPos;


    //호버링했을때 얼마만큼 커질지
    [SerializeField] private float hoverScale = 1.5f;
    //카드 낸다는 y축 기준을 설정하기 위해 빈 GameObject만듬 (이거 외부에서 받아오게해야함)
    private RectTransform _submitCardLine;

    private void Awake()
    {
        originalScale = gameObject.transform.localScale;
        //canvas = gameObject.GetComponentInParent<Canvas>();
        canvas = InGameUIManager.Instance.MainCanvas;
        rect = GetComponent<RectTransform>();
    }
    public void Init(RectTransform transform)
    {
        _submitCardLine = transform;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    //드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        //자신의 턴이 아니면 return
        if (!IsDragAble()) return;
        //원래 부모와 위치를 기억
        originalParent = transform.parent;
        originalPos = rect.anchoredPosition;
        //Grid와 Layout 영향 제거
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling(); //??
    }

    public void OnDrag(PointerEventData eventData)
    {
        //자신의 턴이 아니면 return
        if (!IsDragAble()) return;
        //eventData.delta : 이전프레임과 지금프레임 사이에 포인터가 얼마만큼 움직였는지 (픽셀단위)
        //canvas.scaleFactor : Canvas에 있는 컴포넌트중 Canvas Scaler의 해상도에 따라 UI가 자동 확대/축소중이므로
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //자신의 턴이 아니면 return
        if (!IsDragAble()) return;
        //카드 냈을때
        if (rect.position.y > _submitCardLine.position.y)
        {
            Debug.Log("카드 내짐");
        }
        else //카드를 낸게 아니라면
        {
            //손패로 돌려놓기
            transform.SetParent(originalParent);
            rect.anchoredPosition = originalPos;
        }

    }

    private bool IsDragAble()
    {
        //현재 자신의 ID로 자신의 턴인지 확인한다
        return GameManager.Instance.turnManager.IsMyTurn(GameManager.Instance.playerManager.PlayerID);
    }
}

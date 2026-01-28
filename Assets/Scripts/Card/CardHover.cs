using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour
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

    //드래그했을때 카드가 이동되어야하는지 판별하는 코드
    [SerializeField] CardView cardView;
    //eTargetType targetType;

    private void Awake()
    {
        originalScale = gameObject.transform.localScale;
        canvas = gameObject.GetComponentInParent<Canvas>();
        //canvas = InGameUIManager.Instance.MainCanvas;
        rect = GetComponent<RectTransform>();
    }
    public void Init(RectTransform transform)
    {
        _submitCardLine = transform;
    }
    public void PointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
    }
    public void PointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
    public void PointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
    }
    public void PointerUp(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    //드래그 시작
    public void BeginDrag(PointerEventData eventData)
    {
        //원래 부모와 위치를 기억
        originalParent = transform.parent;
        originalPos = rect.anchoredPosition;
        //Grid와 Layout 영향 제거
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling(); //Hierachy에서 맨뒤로 보냄
    }

    public void Drag(PointerEventData eventData)
    {
        //eventData.delta : 이전프레임과 지금프레임 사이에 포인터가 얼마만큼 움직였는지 (픽셀단위)
        //canvas.scaleFactor : Canvas에 있는 컴포넌트중 Canvas Scaler의 해상도에 따라 UI가 자동 확대/축소중이므로
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void EndDrag(PointerEventData eventData)
    {
        //카드 냈을때
        if (rect.position.y > _submitCardLine.position.y)
        {
            Debug.Log("카드 내짐");
            //현재 낸 카드가 어떤건지 체크해서 제출한다. -> 카드에 대한 정보는? CardView스크립트에서 가져와야하긴함
            //카드를 일정범위이상으로 끌면 화살표 UI가 나오면서 대상을 선택할 수 있음. 공격,힐 같이 단일 선택이면 화살표가 나오게 해야할듯. 전체공격이면 화살표 안나와도됨 
            //전투? 전투를 관리하는 배틀매니저? -> 공격이다. 공격 데미지 계산. 힐이다. 힐 계산 상대를 어떻게 아는데???, 클릭으로 공격할 대상을 지정?
            //CardView cardView = GetComponent<CardView>();
            cardView.ExecuteCommand();
        }
        else //카드를 낸게 아니라면
        {
            //손패로 돌려놓기
            //transform.SetParent(originalParent);
            //rect.anchoredPosition = originalPos;
        }
        //이제 카드를 오브젝트풀로 관리하기때문에 냈던 안냈던간에 원래 부모로 돌려놔야함.
        transform.SetParent(originalParent);
        rect.anchoredPosition = originalPos;

    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardArrorwUI : MonoBehaviour
{
    //카드를 클릭하면 화살표UI가 나와서 타게팅 가능하도록
    UITargetArrow arrow;
    RectTransform rect;

    //카드 데이터를 얻어오기위해서
    [SerializeField] CardView cardView;
    eTargetType targetType;

    //클릭 가능한 대상 타겟팅
    [SerializeField] LayerMask _layer;

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

    public void PointerDown(PointerEventData eventData)
    {
        arrow.Show(rect.position, eventData.position);
    }

    public void Drag(PointerEventData eventData)
    {
        arrow.Show(rect.position, eventData.position);
    }
    public void EndDrag(PointerEventData eventData)
    {
        //드래그가 끝났을때 사용. (Up에서 실제로 드래그가 발생했을때만 호출하기위해서 EndDrag사용)
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit,100f,_layer))
        {
            //Debug.Log("앞에 물체가있다! 이름:" + hit.collider.name);
            //Debug.Log("거리: " + hit.distance);
            PlayerManager enemyPlayer = hit.transform.GetComponentInParent<PlayerManager>();

            if(enemyPlayer != null)
            {
                //Debug.Log("준비 완료");
                //현재 들어있는 카드의 정보를 가지고와서. 그 카드를 hit.transform에 실행해주자
                cardView.ExecuteCommand(enemyPlayer);

            }

        }

    }

    public void PointerUp(PointerEventData eventData)
    {
        arrow.Hide();
    }

    
}


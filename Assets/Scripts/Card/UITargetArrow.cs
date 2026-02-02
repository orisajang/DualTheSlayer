using UnityEngine;

public class UITargetArrow : MonoBehaviour
{
    [SerializeField] RectTransform lineBody;
    [SerializeField] RectTransform arrowHead;

    RectTransform rect;

    Canvas canvas;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false);
    }
    // 월드/스크린 좌표를 Canvas 로컬 좌표로 변환
    private Vector2 WorldToCanvasPosition(Vector2 position)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );
        return localPoint;
    }
    public void Show(Vector2 start, Vector2 end)
    {
        gameObject.SetActive(true);

        Vector2 localStart = WorldToCanvasPosition(start);
        Vector2 localEnd = WorldToCanvasPosition(end);


        //부모를 "시작점"으로 이동
        //rect.position = start;
        rect.anchoredPosition = localStart;

        //목표 지점과 길이를 구한다
        Vector2 dir = localEnd - localStart;
        float length = dir.magnitude; // 두 벡터를 더할때 이동한 결과위치(magnitude)

        // 회전
        //역탄젠트로 각도를 구하고, 오일러 각도로 바꾼뒤 z축으로 회전시킨다(화살표 방향 움직임을 위해서)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; //파이각도인 라디안을 360도 degree로 변환
        rect.rotation = Quaternion.Euler(0, 0, angle);

        // 선 길이 (한쪽으로만 자람)
        lineBody.pivot = new Vector2(0, 0.5f);
        lineBody.anchoredPosition = Vector2.zero;
        lineBody.sizeDelta = new Vector2(length, lineBody.sizeDelta.y); //UI 크기는 비율이아니라 x,y 크기로 바뀜

        // 화살촉 = 정확히 끝점
        arrowHead.pivot = new Vector2(0, 0.5f);
        arrowHead.anchoredPosition = new Vector2(length, 0);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
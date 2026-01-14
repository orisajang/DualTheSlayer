using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUseLine : MonoBehaviour
{
    //디버깅용. 카드를 내는 기준선을 그려주는 스크립트 (런타임에서 Hierachy에 표시됨)
    RectTransform rect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    private void OnDrawGizmos()
    {
        if (rect == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(rect.position.x - 100, rect.position.y, 0), new Vector3(rect.position.x + 100, rect.position.y, 0));
    }
}

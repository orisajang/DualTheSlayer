using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingManager : MonoBehaviour
{
    [SerializeField] UITargetArrow arrow;

    public UITargetArrow Arrow => arrow;

    private void OnEnable()
    {
        GameManager.Instance.SetTargetingManager(this);
    }
    private void OnDisable()
    {
        GameManager.Instance.DeleteTargetManager(this);
    }
}

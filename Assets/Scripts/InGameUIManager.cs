using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : Singleton<InGameUIManager>
{
    [SerializeField] Canvas mainCanvas;

    public Canvas MainCanvas => mainCanvas;
    protected override void Awake()
    {
        isDestroyOnLoad = false;
        base.Awake();

    }

}

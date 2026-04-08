using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    protected override void Awake()
    {
        isDestroyOnLoad = false;
        base.Awake();
    }

    //이펙트도 오브젝트풀로 할수있겠지만 시간상 그냥 Instatiate로 
    public void Play(GameObject effectPrefab, Vector3 pos)
    {
        Instantiate(effectPrefab, pos, Quaternion.identity);
    }


}

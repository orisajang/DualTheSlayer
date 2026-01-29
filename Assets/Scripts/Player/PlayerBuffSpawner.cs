using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffSpawner : MonoBehaviour
{
    [SerializeField] PlayerBuffUI _playerBuffUI;
    [SerializeField] int poolSize = 4;
    ObjPool<PlayerBuffUI> _playerBuffUIPool;

    private void Awake()
    {
        //이 스크립트가 있는 위치에 오브젝트풀 객체를 미리 만들어놓을것임
        _playerBuffUIPool = new ObjPool<PlayerBuffUI>(_playerBuffUI, poolSize, gameObject.transform);
    }

    public PlayerBuffUI GetPlayerBuffUIByPool()
    {
        PlayerBuffUI buffObject = _playerBuffUIPool.GetObject();
        buffObject.Init(this);

        return buffObject;
    }
    public void ReturnPlayerBuffUIToPool(PlayerBuffUI playUI)
    {
        _playerBuffUIPool.ReturnObject(playUI);
    }
}

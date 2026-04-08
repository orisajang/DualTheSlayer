using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConditionSpawner : MonoBehaviour
{
    [SerializeField] PlayerConditionUI _playerConditionUI;
    [SerializeField] int poolSize = 4;
    ObjPool<PlayerConditionUI> _playerConditionUIPool;

    private void Awake()
    {
        //이 스크립트가 있는 위치에 오브젝트풀 객체를 미리 만들어놓을것임
        _playerConditionUIPool = new ObjPool<PlayerConditionUI>(_playerConditionUI, poolSize, gameObject.transform);
    }

    public PlayerConditionUI GetPlayerBuffUIByPool()
    {
        PlayerConditionUI buffObject = _playerConditionUIPool.GetObject();
        buffObject.Init(this);

        return buffObject;
    }
    public void ReturnPlayerBuffUIToPool(PlayerConditionUI playUI)
    {
        _playerConditionUIPool.ReturnObject(playUI);
    }
}

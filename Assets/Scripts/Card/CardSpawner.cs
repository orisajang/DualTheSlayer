using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : Singleton<CardSpawner>
{
    //오브젝트풀로 카드를 미리 생성해주는 스포너
    [SerializeField] CardView _cardPrefab;
    [SerializeField] int poolSize = 10;
    ObjPool<CardView> _cardPool;

    protected override void Awake()
    {
        isDestroyOnLoad = false;
        base.Awake();
        _cardPool = new ObjPool<CardView>(_cardPrefab, poolSize, gameObject.transform);
    }
    //생성 코드
    public CardView GetCardByPool()
    {
        CardView cardBuf = _cardPool.GetObject();
        return cardBuf;
    }
    //오브젝트 반환
    private void ReturnExpPointToPool(CardView expPoint)
    {


        _cardPool.ReturnObject(expPoint);
    }
}

using UnityEngine;

public class DeckPoolSpawner : Singleton<DeckPoolSpawner>
{
    [SerializeField] CardInfoPrefab _prefab;
    [SerializeField] int poolSize = 20;
    //덱과 인벤토리 카드가 생성될 위치
    [SerializeField] Transform _deckParent;
    [SerializeField] Transform _invenParent;

    ObjPool<CardInfoPrefab> _deckObjPool;
    ObjPool<CardInfoPrefab> _invenObjPool;

    protected override void Awake()
    {
        isDestroyOnLoad = false;
        base.Awake();
        _deckObjPool = new ObjPool<CardInfoPrefab>(_prefab, poolSize, _deckParent);
        _invenObjPool = new ObjPool<CardInfoPrefab>(_prefab, poolSize, _invenParent);
    }
    

    //생성 코드 
    public CardInfoPrefab GetDeckCardByPool()
    {
        return _deckObjPool.GetObject();
    }
    public CardInfoPrefab GetInventoryCardByPool()
    {
        return _invenObjPool.GetObject();
    }
    //반환
    public void ReturnDeckCardToPool(CardInfoPrefab card)
    {
        _deckObjPool.ReturnObject(card);
    }
    public void ReturnInvenCardToPool(CardInfoPrefab card)
    {
        _invenObjPool.ReturnObject(card);
    }

}

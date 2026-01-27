using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameNetworkMgr : MonoBehaviourPunCallbacks
{
    //카드를 낼때마다 왼쪽에 어떤 카드 냈는지 표시될곳 (UI)
    [SerializeField] CardView _cardInfoViewPrefab;
    public CardView CardInfoViewPrefab => _cardInfoViewPrefab;

    private Coroutine ShowUsedCardCor;

    public override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.SetInGameNetworkManager(this);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        GameManager.Instance.DeleteInGameNetworkManager(this);
    }
    [PunRPC]
    private void ShowUsedCard_RPC(string imageName, string description, string cost)
    {
        Debug.Log("ShowUsedCard_RPC RPC시작");
        CardView cardInfoView = CardInfoViewPrefab;
        Sprite imageNameToSprite = GameManager.Instance.assetManager.ImageSearchDic[imageName];
        cardInfoView.SetCardResource(imageNameToSprite, description, cost);
        ShowCardInfoUI();
    }
    public void ShowUsedCard(string imageName, string description, string cost)
    {
        photonView.RPC(nameof(ShowUsedCard_RPC), RpcTarget.AllBuffered, imageName, description, cost);
    }
    public void ShowCardInfoUI()
    {
        if(ShowUsedCardCor != null)
        {
            StopCoroutine(ShowUsedCardCor);
        }

        ShowUsedCardCor = StartCoroutine(ShowCardInfoUICor());
    }
    //3초동안 카드 표시하는코루틴
    IEnumerator ShowCardInfoUICor()
    {
        _cardInfoViewPrefab.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        _cardInfoViewPrefab.gameObject.SetActive(false);
    }
}

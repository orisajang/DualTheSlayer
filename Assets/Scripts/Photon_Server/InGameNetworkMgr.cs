using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
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
        if (GameManager.isHaveInstance) GameManager.Instance.DeleteInGameNetworkManager(this);
    }
    [PunRPC]
    private void ShowUsedCard_RPC(string imageName, string description, string cost, string name)
    {
        //Debug.Log("ShowUsedCard_RPC RPC시작");
        CardView cardInfoView = CardInfoViewPrefab;
        Sprite imageNameToSprite = GameManager.Instance.assetManager.ImageSearchDic[imageName];
        cardInfoView.SetCardResourceForShowCard(imageNameToSprite, description, cost, name);
        ShowCardInfoUI();
    }
    public void ShowUsedCard(string imageName, string description, string cost,string name)
    {
        photonView.RPC(nameof(ShowUsedCard_RPC), RpcTarget.AllBuffered, imageName, description, cost, name);
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

    //플레이어가 죽으면 마스터 클라이언트에게 자신과 죽인사람의 ID를 보냄
    public void PlayerDeadNotified(int actorNumber,int attackerActorID)
    {
        //Debug.Log("PlayerDeadNotified호출됨");
        photonView.RPC(nameof(PlayerDead_RPC), RpcTarget.MasterClient, actorNumber, attackerActorID);
    }
    //플레이어 사망했으니까 게임결과에 따라 exp를 서로 올려줌
    [PunRPC]
    public void PlayerDead_RPC(int deadPlayerActorNumber, int attackerActorID)
    {
        //마스터클라이언트가 아니면 return
        if (!PhotonNetwork.IsMasterClient) return;

        //Debug.Log("마스터클라이언트에서 죽은사람과 승리자에게 RPC를 보내려고함");
        //플레이어가 죽었다. 이 사실을 마스터 클라이언트에게만 RPC로 보낸다.
        //죽은사람은 exp 50증가, 이긴사람은 exp 100증가시키면 된다. 
        int defeatPlayerAddExp = 50;
        int victoryPlayerAddExp = 100;
        foreach(Player ply in PhotonNetwork.PlayerList)
        {
            //만약 죽은사람이었다면 (패배한사람)
            if(ply.ActorNumber == deadPlayerActorNumber)
            {
                //Debug.Log("마스터클라이언트에서 죽은사람에게 RPC를 보냄");
                photonView.RPC(nameof(AddExpValue), ply, defeatPlayerAddExp);
            }
            else if( ply.ActorNumber == attackerActorID) //만약 공격자였다면 (승리한사람)
            {
                //Debug.Log("마스터클라이언트에서 이긴사람에게 RPC를 보냄");
                photonView.RPC(nameof(AddExpValue), ply, victoryPlayerAddExp);
            }
        }

        photonView.RPC(nameof(ShowGameResultPanel_RPC), RpcTarget.All, deadPlayerActorNumber, attackerActorID, defeatPlayerAddExp, victoryPlayerAddExp);
    }
    //올려야되는 대상들만 RPC로 아래 이벤트 받고 DB에 결과 저장
    [PunRPC]
    public void AddExpValue(int addExpValue)
    {
        //Debug.Log($"마스터클라이언트에서 RPC를 받았습니다 {addExpValue}");
        GameManager.Instance.SavePlayerData(addExpValue);
    }
    [PunRPC]
    public void ShowGameResultPanel_RPC(int deadPlayerActorNumber, int attackerActorID, int defeatPlayerAddExp, int victoryPlayerAddExp)
    {
        //Debug.Log("결과패널 열기 시작");
        //설정 안되어있으면 관전자라는 의미이므로 
        string gameResult = "";
        if(PhotonNetwork.LocalPlayer.ActorNumber == deadPlayerActorNumber) //패배한사람
        {
            gameResult = "Defeate!!";
        }
        else if(PhotonNetwork.LocalPlayer.ActorNumber == attackerActorID) //승리한사람
        {
            gameResult = "Victory!!";
        }
        else //관전자라는 의미
        {
            gameResult = "GameResult";
        }
        string resultInfo = $"{deadPlayerActorNumber} 패배. {defeatPlayerAddExp}의 경험치 획득 \n" +
            $"{attackerActorID} 승리. {victoryPlayerAddExp}의 경험치 획득";
        GameManager.Instance.ShowGameResultPanel(gameResult, resultInfo);
    }

    //파이어베이스를 통해 카드ID값을 저장
    public void AddPlayerInventoryCard(string cardId)
    {
        DatabaseReference dbRef = NetworkEventManager.Instance.GetInvenRef();

        //1개만 추가하면 되므로 이렇게 고유 키값을 추가하면 된다고 한다
        string key = dbRef.Push().Key;
        dbRef.Child(key).SetValueAsync(cardId);
    }
}

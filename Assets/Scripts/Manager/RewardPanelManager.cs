using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanelManager : MonoBehaviour
{
    [SerializeField] Button _leaveRoomButton;
    //카드가 생성될위치가 저장된 게임오브젝트
    [SerializeField] GameObject _cardSpawnPositionParent;
    //생성될 카드UI프리팹
    [SerializeField] CardRewardUI _cardRewardUI;
    List<RectTransform> cardSpawnPosition;

    //생성된 프리팹들에 접근하기위해
    List<CardRewardUI> _cardRewardUIList = new List<CardRewardUI>();
    private CardRewardUI _selectedCardReward;

    public void OnEnable()
    {
        _leaveRoomButton.onClick.AddListener(GetCardAndLeaveRoom);
    }
    //DB의 현재 플레이어 ID - 인벤토리에 카드 1개 등록
    private void GetCardAndLeaveRoom()
    {
        if (_selectedCardReward == null) return;
        //선택한 카드 추가
        GameManager.Instance.inGameNetworkMgr.AddPlayerInventoryCard(_selectedCardReward.CardId);
        //방 떠나기
        PhotonNetwork.LeaveRoom();
    }


    public void SetAndStartRewardProgress(int cardCount)
    {
        Debug.Log("SetAndStartRewardProgress 진입");
        cardSpawnPosition = _cardSpawnPositionParent.GetComponentsInChildren<RectTransform>().Where(a => a.gameObject != _cardSpawnPositionParent).ToList();
        //cardCount => 몇장의 카드가 등장해야하는지
        if (cardCount > cardSpawnPosition.Count) 
        {
            Debug.LogError($"표시할수있는 카드는 {cardSpawnPosition.Count}개 입니다 {cardCount}개를 표시하려고해서 에러!");
        }

        //카드딕셔너리에 대한 정보를 받아서 처리하거나? 혹은
        //cardCount만큼 for문을 돌려서 어떤 카드ID가 필요한지 체크. 단, 중복이 뜨면 안된다; -> List로 변경?;
        List<string> cardIdList = GameManager.Instance.CardSODic.Keys.ToList();
        List<string> selectIdList = new List<string>();

        //랜덤으로 선택
        for(int i=0; i< cardCount; i++)
        {
            int randomIndex = Random.Range(0, cardIdList.Count);
            string selectedId = cardIdList[randomIndex];

            selectIdList.Add(selectedId);
            cardIdList.RemoveAt(randomIndex);
        }
        //선택 다 끝났으니까 이제 딕셔너리에서 매칭해서 만들어주자
        int idCount = selectIdList.Count;
        for(int spawnIndex =0; spawnIndex < idCount; spawnIndex++)
        {
            //어떤 카드들이 선택되었는지 카드ID로 매칭
            CardSO cardSO = GameManager.Instance.CardSODic[selectIdList[spawnIndex]]; //절대 CardSO안의 데이터 바꾸지말것.
            //보상 카드UI를 설정한다
            CardRewardUI rewardUI = Instantiate(_cardRewardUI, this.gameObject.transform.position, Quaternion.identity,
                this.gameObject.transform).GetComponent<CardRewardUI>();
            rewardUI.GetComponent<RectTransform>().anchoredPosition = cardSpawnPosition[spawnIndex].anchoredPosition;
            rewardUI.SetCardRewadUI(cardSO.CardImage, cardSO.Description, cardSO.Cost.ToString(), selectIdList[spawnIndex]);
            rewardUI.OnClicked += ActiveSelecteCardOutline;
            _cardRewardUIList.Add(rewardUI);
        }

        //패널 활성화
        gameObject.SetActive(true);

    }
    //선택한 카드의 외각선만 활성화시켜주기위해서
    private void ActiveSelecteCardOutline(CardRewardUI cardRewardUI)
    {
        //현재 선택한 카드 UI
        _selectedCardReward = cardRewardUI;
        //현재 선택한 카드 UI의 외각선만 활성화
        foreach (CardRewardUI rewardUI in _cardRewardUIList)
        {
            if (rewardUI == cardRewardUI){ rewardUI.SetOutLineUI(true); }
            else { rewardUI.SetOutLineUI(false); }
        }
        _leaveRoomButton.enabled = true;
    }

    private void OnDisable()
    {
        //이벤트 구독했던거를 전부 해제해주자
        foreach(CardRewardUI rewardUI in _cardRewardUIList)
        {
            rewardUI.OnClicked -= ActiveSelecteCardOutline;
        }
    }

}

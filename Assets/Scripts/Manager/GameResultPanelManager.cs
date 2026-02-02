using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultPanelManager : MonoBehaviour
{
    //게임결과 패널
    [SerializeField] GameObject _resultPanel;
    //게임의 결과와 상세 결과 표시할 텍스트
    [SerializeField] TextMeshProUGUI _gameResultText;
    [SerializeField] TextMeshProUGUI _resultInfoText;
    //방에서 나가는 버튼
    [SerializeField] Button _leaveRoomButton;

    //보상확인 패널 활성화를 위해
    [SerializeField] RewardPanelManager _rewardPanel;

    //패널을 활성화
    public void ActiveResultPanelAndSetInfo(string gameResult, string gameInfo)
    {
        //패널 활성화
        _resultPanel.SetActive(true);
        //
        _gameResultText.text = gameResult;
        _resultInfoText.text = gameInfo;
    }

    public void OnEnable()
    {
        //버튼 이벤트 등록 및 게임매니저에 자신을 등록
        //_leaveRoomButton.onClick.AddListener(() => PhotonNetwork.LeaveRoom());
        _leaveRoomButton.onClick.AddListener(OnCheckRewardButtonClick);
        GameManager.Instance.SetGameResultPanelManager(this);
    }
    public void OnDisable()
    {
        _leaveRoomButton.onClick.RemoveAllListeners();
        if(GameManager.isHaveInstance) GameManager.Instance.DeleteGameResultPanelManager(this);
    }
    public void OnCheckRewardButtonClick()
    {
        this.gameObject.SetActive(false);
        //일단 무조건 카드 3장이 보이도록
        _rewardPanel.SetAndStartRewardProgress(3);
    }

}

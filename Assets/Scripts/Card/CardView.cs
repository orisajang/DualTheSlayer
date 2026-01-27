using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour, ICardMVPView
{
    [SerializeField] Image cardImage;
    [SerializeField] TextMeshProUGUI cardDescription;

    //MVP관련 설정
    CardPresenter cardPresenter;

    //현재 카드의 데이터
    CardSOClass cardClass;

    public void Init(CardInstance instance)
    {
        //카드 모델은 생성만하고 Presenter에 담자.
        CardModel cardModel = new CardModel();
        cardModel.Init(instance);
        cardPresenter = new CardPresenter(cardModel, this);
        //카드 이미지, 설명을 갱신
        cardPresenter.UpdateCardUI();
        //현재 카드의 데이터 설정
        cardClass = cardPresenter.ReturnCardData();
    }

    //Presenter에서 받아온 정보로 UI 갱신
    public void SetCardResource(Sprite sprite, string text)
    {
        cardImage.sprite = sprite;
        cardDescription.text = text;
    }
    public eTargetType GetTargetType()
    {
        eTargetType type = cardPresenter.GetInputType();
        return type;
    }

    //임시용-> 매개변수 player로 바꿔야함. 적이 들어오면 Presenter에 해당정보를 보내줘서 알아서 현재 카드를 실행하게 한다
    public void ExecuteCommand(PlayerManager enemyPlayer)
    {
        //무조건 자신한테 사용하는 버프 효과 (쉴드, 데미지증가, 힘증가)
        //사용자, 타겟, 
        int player = GameManager.Instance.turnManager.CurrentPlayerId; //사용자, 타겟
        PlayerManager myPlayer = GameManager.Instance.playerManager;
        CardTargetInfoClass cardTargetInfoClass = new CardTargetInfoClass(myPlayer, enemyPlayer,cardClass.Cost);
        cardPresenter.ExecuteCard(this, cardTargetInfoClass);

        //CardTargetInfoClass cardTargetInfoClass = new CardTargetInfoClass(myPlayer, myPlayer);
        //cardPresenter.ExecuteCard(enemy, this, );
    }
    //자신, 혹은 전체에 사용
    public void ExecuteCommand()
    {
        //무조건 자신한테 사용하는 버프 효과 (쉴드, 데미지증가, 힘증가)
        //사용자, 타겟, 
        int player = GameManager.Instance.turnManager.CurrentPlayerId; //사용자, 타겟
        PlayerManager myPlayer = GameManager.Instance.playerManager;
        CardTargetInfoClass cardTargetInfoClass = new CardTargetInfoClass(myPlayer, myPlayer, cardClass.Cost);
        cardPresenter.ExecuteCard(this, cardTargetInfoClass);
    }
    //현재 자신의 카드가 사용가능한 상태인지 체크 (사용불가능하면 코스트Text가 빨간색으로 할 예정)
    public bool IsUseableCard()
    {
        //현재 행동력과 카드의 비용을 체크해서 카드를 사용할 수 있는 상태인지 체크한다.
        int playerEnergy = GameManager.Instance.turnManager.CurrentTurnPlayer.currentEnergy;
        if(cardClass.Cost > playerEnergy)
        {
            Debug.Log($"행동력이 더작다 카드를 내면안됨 ");
            return false;
        }
        else
        {
            Debug.Log($"카드내도됩니다~~");
            return true;
        }
    }
}



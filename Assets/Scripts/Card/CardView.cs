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

    public void Init(CardInstance instance)
    {
        //카드 모델은 생성만하고 Presenter에 담자.
        CardModel cardModel = new CardModel();
        cardModel.Init(instance);
        cardPresenter = new CardPresenter(cardModel, this);
        //카드 이미지, 설명을 갱신
        cardPresenter.UpdateCardUI();
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
        CardSOClass cardClass = cardPresenter.ReturnCardData();
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
        CardSOClass cardClass = cardPresenter.ReturnCardData();
        CardTargetInfoClass cardTargetInfoClass = new CardTargetInfoClass(myPlayer, myPlayer, cardClass.Cost);


        cardPresenter.ExecuteCard(this, cardTargetInfoClass);
    }
}



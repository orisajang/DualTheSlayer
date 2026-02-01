using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CardInstance
{
    //CardSO의 데이터를 받아서 클래스로 만들어주기 위해 사용
    public string CardID { get; private set; }
    public string CardName { get; private set; } //카드 이름
    public int Cost { get; private set; } //카드 비용
    public eCardType CardType { get; private set; } //카드 타입
    public Sprite CardImage { get; private set; }//카드 이미지
    public string Description { get; private set; } //카드 설명
    public eTargetType TargetAble { get; private set; } //타게팅 가능한 스킬인지, 실행되면 무엇을 해야하는지
    //실행할 행동SO 리스트 (절대 수정하면 안됨. 가져오는것까진 가능할듯)
    private List<CardExecuteSO> _executeSOList;
    public IReadOnlyList<CardExecuteSO> ExecuteSOList => _executeSOList;

    //카드 원본 데이터 (절대 수정하면 안됨. 가져오는것까진 가능할듯)
    private CardSO CardDataOrigin;
    private string cardInitDescription;
    //현재 카드의 UI
    public CardView CardView { get; private set; }
    public event Action OnCardDataUpdated;

    public CardInstance(CardSO data)
    {
        CardID = data.CardId;
        CardName = data.CardName;
        Cost = data.Cost;
        //CardType = data.CardType;
        CardImage = data.CardImage;
        Description = data.Description;
        TargetAble = data.TargetAble;
        _executeSOList = data.ExecuteSO; //이거 무조건 얕은복사이므로 안에있는 필드값 수정하면안됨.

        CardDataOrigin = data;
        //카드 초기 설명 체크
        MakeCardDescription(_executeSOList);

    }

    public void SetCardInstanceData(string description)
    {
        Description = description;
        //카드 데이터가 변경되었다는 알림 발생(UI를 갱신시키기 위해)
        OnCardDataUpdated?.Invoke();
    }
    //카드 데이터에 있는 Execute데이터를 확인하며 카드 설명 Text에 어떤값이 표시되어야하는지 체크
    private void MakeCardDescription(List<CardExecuteSO> executeSOList)
    {
        StringBuilder sb = new StringBuilder();
        foreach(CardExecuteSO executeSO in executeSOList)
        {
            //sb에 카드 설명들을 다 쌓아둠
            sb.Append(executeSO.CardInitDescription());
        }
        Description = sb.ToString();
    }

    //자신의 카드가 가지고있는 행동들을 모두 실행시킨다
    public bool ExecuteAll(CardTargetInfoClass cardTargetInfoClass)
    {
        bool isAllSuccess = false;
        foreach (CardExecuteSO cardExecuteSO in _executeSOList)
        {
            isAllSuccess = cardExecuteSO.CanExecute(cardTargetInfoClass);
            if (isAllSuccess == false)
            {
                Debug.LogWarning($"카드 행동중에서 실행하지 못하는 행동이 있어 취소합니다");
                return false; //실행불가면 바로 false return
            }
        }

        //실행 가능하므로 이제 모두 실행시켜준다
        foreach (CardExecuteSO cardExecuteSO in _executeSOList)
        {
            cardExecuteSO.Execute(cardTargetInfoClass);
        }
        //전부 실행완료한다음에 해야할 작업들이 있는지 체크 (자기자신에게 카드 낸다음에 부여되는 효과들이 있는지)
        Debug.Log("카드 행동들 실행완료. 카드낸후에 해야할것들있는지 확인합니다");
        cardTargetInfoClass.UseCardPlayer.AfterUseCardBehavior(cardTargetInfoClass.cardInstanceData.Cost);

        return true;  //실행다했으면 true보냄 
    }


}
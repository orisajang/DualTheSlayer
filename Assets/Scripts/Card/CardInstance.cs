using System.Collections.Generic;
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
    private List<CardExecuteSO> ExecuteSOList;

    //카드 원본 데이터 (절대 수정하면 안됨. 가져오는것까진 가능할듯)
    private CardSO CardDataOrigin;

    public CardInstance(CardSO data)
    {
        CardID = data.CardId;
        CardName = data.CardName;
        Cost = data.Cost;
        CardType = data.CardType;
        CardImage = data.CardImage;
        Description = data.Description;
        TargetAble = data.TargetAble;
        ExecuteSOList = data.ExecuteSO; //이거 무조건 얕은복사이므로 안에있는 필드값 수정하면안됨.

        CardDataOrigin = data;
    }
    //자신의 카드가 가지고있는 행동들을 모두 실행시킨다
    public bool ExecuteAll(CardTargetInfoClass cardTargetInfoClass)
    {
        bool isAllSuccess = false;
        foreach (CardExecuteSO cardExecuteSO in ExecuteSOList)
        {
            isAllSuccess = cardExecuteSO.CanExecute(cardTargetInfoClass);
            if (!isAllSuccess)
            {
                Debug.LogWarning($"카드 행동중에서 실행하지 못하는 행동이 있어 취소합니다");
                return false; //실행불가면 바로 false return
            }
        }

        //실행 가능하므로 이제 모두 실행시켜준다
        foreach (CardExecuteSO cardExecuteSO in ExecuteSOList)
        {
            cardExecuteSO.Execute(cardTargetInfoClass);
        }
        return true;  //실행다했으면 true보냄 
    }


}
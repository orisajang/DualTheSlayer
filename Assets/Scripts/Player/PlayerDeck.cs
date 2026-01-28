using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    //플레이어가 가지고있는 카드의 종류를 관리하는 곳
    List<CardSO> cardList; //절대 안의값 수정하면 안됨. 카드 원본데이터임
    //원본 덱 데이터는 저장해둔 상태로 계속 꺼내서 사용한다.
    List<CardInstance> currentCardList = new List<CardInstance>();

    int currentIndex = 0;

    public void SetCard(List<CardSO> deck)
    {
        cardList = deck;
    }

    public CardInstance GetCard()
    {
        //초기 설정 or 덱을 한번씩 다 썼으면 다시 덱 리스트를 채워준다
        if (currentCardList.Count == 0|| currentCardList.Count <= currentIndex)
        {
            currentCardList.Clear();

            foreach(CardSO cardData in cardList)
            {
                CardInstance cardInstance = new CardInstance(cardData);
                currentCardList.Add(cardInstance);
            }
            //피셔에이츠로 덱을 섞어준다
            ShuffleCard(currentCardList);
            currentIndex = 0;
        }
        //하나씩 return 해준다
        if(currentCardList.Count <= 0)
        {
            Debug.LogError("에러발생. 카드를 줄수없습니다"); 
        }
        return currentCardList[currentIndex++];
        
    }
    //랜덤을 위해 피셔 에이츠방식을 이용해서 덱을 섞어주고 가져온다
    private void ShuffleCard(List<CardInstance> cards)
    {
        for (int count = cards.Count - 1; count > 0; count--)
        {
            int index = Random.Range(0, count + 1);
            CardInstance temp = cards[count];
            cards[count] = cards[index];
            cards[index] = temp;
        }
    }

}

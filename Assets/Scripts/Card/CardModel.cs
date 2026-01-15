using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel : MonoBehaviour
{
    //현재 코스트 정보 등 게임을하며 바뀐 카드 데이터를 저장하는곳
    CardInstance cardInstance;
    public CardInstance CardInstance => cardInstance;

    public void Init(CardInstance instance)
    {
        cardInstance = instance;
    }
}

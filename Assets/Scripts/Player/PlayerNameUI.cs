using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerNameText;

    public void SetPlayerName(string name, bool isMine)
    {
        _playerNameText.text = name;
        //내꺼면 초록색으로 표시
        if(isMine)
        {
            _playerNameText.color = Color.green;
        }
    }
}

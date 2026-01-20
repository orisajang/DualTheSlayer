using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoUIPrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI teamInfoText;

    public TextMeshProUGUI PlayerNameText => playerNameText;
    public TextMeshProUGUI TeamInfoText => teamInfoText;

    public void SetPlayerInfoUI(string name, string teamInfo)
    {
        playerNameText.text = name;
        teamInfoText.text = teamInfo;
    }
}

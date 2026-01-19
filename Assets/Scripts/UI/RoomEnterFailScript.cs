using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEnterFailScript : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Button _closeButton;
    [SerializeField] TextMeshProUGUI _errorInfoText;

    public void Init(string errorMessage)
    {
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(ClosePanel);

        _errorInfoText.text = errorMessage;
    }

    private void ClosePanel()
    {
        panel.SetActive(false);
    }
}

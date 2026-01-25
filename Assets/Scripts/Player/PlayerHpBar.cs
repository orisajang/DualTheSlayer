using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] Image _hpBarImage;
    [SerializeField] TextMeshProUGUI _hpText;

    public void UpdateHPBarInfo(float hpRatio,int currentHp)
    {
        _hpBarImage.fillAmount = hpRatio;
        _hpText.text = currentHp.ToString();
    }
}

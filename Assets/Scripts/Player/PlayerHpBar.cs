using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] Image _hpBarImage;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _shieldText;

    public void UpdateHPBarInfo(float hpRatio,int currentHp,int shield)
    {
        _hpBarImage.fillAmount = hpRatio;
        _hpText.text = currentHp.ToString();

        //쉴드량
        if(shield <=0) _shieldText.text = "";
        else _shieldText.text = shield.ToString();
    }
}

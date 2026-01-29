using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum eBuffType
{
    None, DotHealing ,Bleeding
}

public class PlayerBuffUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buffAmountText;
    [SerializeField] private Image _buffImage;
    [SerializeField] private TextMeshProUGUI _buffDurationText;

    //생성될 UI프리팹
    [SerializeField] private GameObject _playerBuffPrefab;

    //버프값과 버프 지속시간
    private int _amount;
    private int _duration;
    private eBuffType _buffType;

    //이 객체를 생성해준 부모(Spawner)를 기억해둠. 반환할때 반환시키기 위해)
    private PlayerBuffSpawner _buffSpawner;

    public void Init(PlayerBuffSpawner spawner)
    {
        _buffSpawner = spawner;
    }

    //이미지는 잠시 안쓴다 가정하자
    public void SetBuffInfo(int buffAmount, int buffDuration, eBuffType type)
    {
        _amount = buffAmount;
        _duration = buffDuration;
        _buffType = type;

        //UI요소 갱신
        UpdateUIElement();
    }

    public void AddBuffInfo(int buffAmount, int buffDuration, eBuffType buffType)
    {
        if(buffType != _buffType)
        {
            Debug.LogError($"버프 타입이 다릅니다 뭔가 에러");
            return;
        }

        _amount += buffAmount;
        _duration += buffDuration;


        //UI요소 갱신
        UpdateUIElement();
    }
    private void UpdateUIElement()
    {
        _buffAmountText.text = _amount.ToString();
        _buffDurationText.text = _duration.ToString();
        //이미지는 아직 잘 모르겠음
    }
    //버프효과 1회 발동 (duration 1회 차감)
    public bool ActivateBuffOnce()
    {
        //버프가 끝나면 true보내고 안끝났으면 false보냄
        _duration -= 1;
        UpdateUIElement();
        if (_duration <= 0)
        {
            Debug.Log($"{_buffType.ToString()}의 버프가 끝났습니다");
            //Destroy(gameObject);
            _buffSpawner.ReturnPlayerBuffUIToPool(this);
            return true;
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDamageString : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private float textDuration = 3; //텍스트 표시 지속시간
    [SerializeField] private float speed = 10;
    private float _currentTime;

    RectTransform _rectTransform;
    //설정과 동시에 필요한 것들을 찾아서 실행한다
    public void SetAndStartDamageText(int amount, bool isDamageText)
    {
        //텍스트 색성 설정
        if (isDamageText) { _damageText.color = Color.red; }
        else { _damageText.color = Color.green; }

        _damageText.text = amount.ToString();
        _rectTransform = GetComponent<RectTransform>();
        _currentTime = textDuration;

        StartCoroutine(ShowDamageCor());
    }
    // N초동안 살아있다가 스스로 삭제
    IEnumerator ShowDamageCor()
    {
        while(_currentTime > 0)
        {
            _rectTransform.anchoredPosition += (Vector2.up * speed * Time.deltaTime);
            _currentTime -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}

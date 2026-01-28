using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerLevelData
{
    //[System.Serializable]는 public과 [SerializeField] 붙은 private필드만 저장됨 
    [SerializeField] private int _level = 1;
    [SerializeField] private int _exp;

    public int Level => _level;
    public int Exp => _exp;

    //레벨업이 되는 exp 기준
    int limitExpValue = 100;

    public void AddExpAndCheckLevelUp(int expAddValue)
    {
        int totalExp = _exp + expAddValue;
        int playerLvl = _level;
        //레벨업 해야하면 해당 기준으로 수치를 맞춰줌
        if (totalExp >= limitExpValue)
        {
            playerLvl += totalExp / limitExpValue; // 120 / 100 = 1
            totalExp = totalExp % limitExpValue;
        }
        _level = playerLvl;
        _exp = totalExp;
    }
}

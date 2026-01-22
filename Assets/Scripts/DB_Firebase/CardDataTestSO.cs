using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName = "Card/CardSO Make", fileName = "CardSOTestData")]
public class CardDataTestSO : ScriptableObject
{
    [SerializeField] private string cardID;
    [SerializeField] private string cardName;
    [SerializeField,Range(0,10)] private int cost;
    [SerializeField] private string summary;
    [SerializeField] private Sprite image;

    public string CardId => cardID;
    public string CardName => cardName;
    public int Cost => cost;
    public string Summary => summary;
    public Sprite Image => image;
}

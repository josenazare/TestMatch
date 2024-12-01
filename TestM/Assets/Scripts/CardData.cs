using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardData
{
    public int CardSO_ID; 
    public bool isFlipped;
    public bool isMatched;
  //  public Sprite CardImageData;
}

[System.Serializable]
public class CardSaveData
{
    public List<CardData> Cards = new List<CardData>();
}
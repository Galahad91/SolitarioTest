using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "CardComponents")]
public class CardData : ScriptableObject
{ 
    public Sprite[] Suits;
    public Sprite[] Num;
    public Sprite back;
    public Sprite front;

    public GameObject card;

}

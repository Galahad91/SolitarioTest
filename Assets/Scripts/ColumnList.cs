using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnList : MonoBehaviour
{
    public int cardAtStart;
    public Stack<Card> cardInColumn;

    private void Awake()
    {
        cardInColumn = new Stack<Card>();
    }

 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackList : MonoBehaviour
{
    public SUIT stackSuit;
    public Stack<Card> cardInStack;

    private void Awake()
    {
        cardInStack = new Stack<Card>();
    }

 
}

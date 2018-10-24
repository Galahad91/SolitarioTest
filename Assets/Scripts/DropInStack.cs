using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropInStack : MonoBehaviour, IDropHandler
{
    StackList thisStack;
    GMController gm;
    private void Awake()
    {
        thisStack = GetComponent<StackList>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GMController>();
    }

    public void OnDrop(PointerEventData pointer)
    {
        Card card = pointer.pointerDrag.GetComponent<Card>();
        //check if the move is legit
        if(thisStack.cardInStack.Count == 0)
        {

            if(card.GetSuit() == thisStack.stackSuit && card.GetNumber() == 1)
            {

                gm.RearrangeOldColumn(card);
                card.drag.currentParent = thisStack.transform;
                thisStack.cardInStack.Push(card);
                gm.AddScore(gm.scoreStack);// add score
                gm.AddMoves();
            }
        }
        else
        {
            if (card.GetSuit() == thisStack.stackSuit && card.GetNumber() == (thisStack.cardInStack.Peek().GetNumber() + 1))
            {
                gm.RearrangeOldColumn(card);
                card.drag.currentParent = thisStack.transform;
                thisStack.cardInStack.Push(card);
                gm.AddScore(gm.scoreStack); //add score
                gm.AddMoves();
            }
        }
    }

}

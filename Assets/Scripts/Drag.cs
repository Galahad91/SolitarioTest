using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler
{
    Card thisCard;
    GMController gm;
    ColumnList currentColumn;
    Transform originalParent;
    public Transform currentParent;  
    
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GMController>();
        thisCard = GetComponent<Card>();
    }

    public void OnBeginDrag(PointerEventData pointer)
    {
        if (thisCard.canBePicked && gm.GetPrepDone())
        {   
            // save the reference to the current column
            originalParent = transform.parent;
            currentParent = originalParent;
            
            // get the column list component to be able to check if this card is the last one in the current column
            currentColumn = currentParent.GetComponent<ColumnList>();
            // if is not the last one takes all the other cards and set them as child of the current card to be able to move them togheter
            if (currentColumn != null)
            {
                if (currentColumn.cardInColumn.Peek().gameObject != thisCard.gameObject)
                {
                    while (currentColumn.cardInColumn.Peek().gameObject != thisCard.gameObject)
                    {   //record moving cards
                        gm.numCardMoved++;
                        gm.record.Add(new CardRecord(currentColumn.cardInColumn.Peek(), currentColumn.cardInColumn.Peek().transform.parent, gm.numCardMoved, gm.GetScore(),0,0));

                        gm.movingCards.Push(currentColumn.cardInColumn.Peek());
                        currentColumn.cardInColumn.Pop();
                        gm.movingCards.Peek().transform.SetParent(transform);

                    }
                }
            }

            gm.movingCards.Push(thisCard);
            // record this card movements
            gm.numCardMoved++;
            CardRecord newRecord = new CardRecord(thisCard, originalParent, gm.numCardMoved, gm.GetScore(),0,0); 
            gm.record.Add(newRecord);
            // change parent to background ***THE PARENT MUST BE CHANGED AFTER THE MOVEMENT HAS BEEN RECORDED***
            transform.SetParent(gm.background); 
            // blocks the raycast target of all the other interactable cards to avoid multiple movements
            for (int i = 0; i < gm.deck.Length; i++)
            {
                if (gm.deck[i].canBePicked)
                {
                    gm.deck[i].EnableRaycast(false);
                }
            }
            //block raycast target on other buttons
            gm.undoButton.raycastTarget = false;
            gm.optionsbutton.raycastTarget = false;
            gm.exitButton.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData pointer)
    {
        if (thisCard.canBePicked && gm.GetPrepDone())
        {   //follow the pointer position    
            transform.position = pointer.position;
        }
    }

    public void OnEndDrag(PointerEventData pointer)
    {
        if (thisCard.canBePicked && gm.GetPrepDone())
        {   //assign the new parent to the card and update the column list component
            transform.SetParent(currentParent);
            //if the move is invalid delete records
            if(currentParent == originalParent)
            {
                while(gm.numCardMoved > 0)
                {
                    gm.record.RemoveAt(gm.record.Count-1);
                    gm.numCardMoved--;
                }
            }
            gm.numCardMoved = 0;
            currentColumn = currentParent.GetComponent<ColumnList>();
            //unload all the cards from the temporary stack
            gm.movingCards.Pop();
            if (currentColumn != null)
            {
                while (gm.movingCards.Count > 0)
                {
                    currentColumn.cardInColumn.Push(gm.movingCards.Peek());
                    currentColumn.cardInColumn.Peek().transform.SetParent(currentParent);
                    gm.movingCards.Pop();
                }
            }
            // if the parent is a stack it will disable the interactions 
            if (currentParent.GetComponent<StackList>() != null)
            {
                thisCard.canBePicked = false;
            }
          
            // enable raycast target for all the interactable cards
            for (int i = 0; i < gm.deck.Length; i++)
            {
                if (gm.deck[i].canBePicked)
                {
                    gm.deck[i].EnableRaycast(true);
                }
            }

            //enable raycast target on other buttons
            gm.undoButton.raycastTarget = true;
            gm.optionsbutton.raycastTarget = true;
            gm.exitButton.raycastTarget = true;
        }
        
    }

}

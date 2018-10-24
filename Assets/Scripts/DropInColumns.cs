using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropInColumns : MonoBehaviour, IDropHandler
{
    ColumnList thisColumn;  //reference to the columnList script
    GMController gm;
    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GMController>();
        thisColumn = GetComponent<ColumnList>();
    }

    public void OnDrop(PointerEventData pointer)
    {   
        Card card = pointer.pointerDrag.GetComponent<Card>();
        //check if the stack has cards in it and if the move is legit
        if ( thisColumn.cardInColumn.Count > 0)
        { 
            //check if the dragged card is of opposite color as the last one in the column and has an immediate lesser valor
            if (card.GetCardColor() != thisColumn.cardInColumn.Peek().GetCardColor() 
                && card.GetNumber() == (thisColumn.cardInColumn.Peek().GetNumber() - 1)
                && card.GetNumber() != 1) 
            {
                gm.RearrangeOldColumn(card);

                gm.UpdateCurrentNewColumn(card, thisColumn);
                gm.AddMoves();

            }
        }
        // if the column is empty check if the card is a king
        else if (thisColumn.cardInColumn.Count == 0)
        {
            if (card.GetNumber() == gm.data.Num.Length)
            {
                gm.RearrangeOldColumn(card);

                gm.UpdateCurrentNewColumn(card, thisColumn);
                gm.AddMoves(); 
            }
        }
    }   
         
}

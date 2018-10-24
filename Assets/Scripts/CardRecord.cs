using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRecord : MonoBehaviour
{
    public Card currentCard;
    public Transform originalParent;
    public bool raycastEnabled;                      //  card parameters at the moment of the record
    public bool wasCovered;                          //    
    public bool couldBePicked;                       //
    public bool wasInStock;                          //

    public int numCardsInMove;                       // how many card the player is moving in one move (column drag)
    public int score;                                // game score at the moment of the record
    public bool prevCardCovered;                     //check if the previous card in the column was covered or not
    public int stockIndex;                           //index in the stock list 
    public int stockReset;                           // how many times the stock was rearranged at the time of the record

    public CardRecord(Card card, Transform parent, int num, int s, int index, int stockCount)
    {
        currentCard = card;
        originalParent = parent;
        numCardsInMove = num;
        score = s;
        stockIndex = index;
        stockReset = stockCount;

        raycastEnabled = currentCard.backImage.raycastTarget;
        wasCovered = currentCard.GetIsCovered();  
        couldBePicked = currentCard.canBePicked;
        wasInStock = currentCard.isInStock;

        //if the parent was a column gets the covered status of the penultimate card in the column
        ColumnList col = originalParent.GetComponent<ColumnList>();
        if(col!= null && col.cardInColumn.Count > 1)
        {
            prevCardCovered = originalParent.GetChild(originalParent.childCount - 2).GetComponent<Card>().GetIsCovered();
        }
       
       

    }
}

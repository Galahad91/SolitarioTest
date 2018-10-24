using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StockHandler : MonoBehaviour, IPointerClickHandler
{
    GMController gm;
    public bool uncoveredCardsRearranged = true;
    public bool canClick = true;
    public float timeOfTravel = 0;  
    public Card first;             //reference to the first uncovered card showing
    public Card second;            //reference to the second uncovered card showing
    public Card third;             //reference to the third uncovered card showing

    [HideInInspector] public int stockReset = 0;

    private void Awake() 
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GMController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(gm.GetPrepDone() && canClick && uncoveredCardsRearranged)  
        {
            canClick = false;
            if(gm.stock.Count > 0)  //if the stock has cards in it put the first on the tabletop
            {
                if (!gm.GetDrawMode()) // if it's regular draw mode
                {
                    StartCoroutine(SetUncoveredCards(gm.stock, true));
                }
                else // if it's draw 3 mode on
                {
                    StartCoroutine(SetUncoveredCardsDraw3(gm.stock, true));
                }
                gm.numCardMoved=0;
                gm.AddMoves(); 
            }
            else if (gm.stock.Count == 0)  // if the stock is empty then puts back all the cards in it
            {
                StartCoroutine(SetStockCard(gm.uncoveredCards, gm.stock, gm.stockPosition.transform,false));                     
            }
        }
    }

    IEnumerator SetStockCard(Stack<Card> stackA, List<Card> stackB, Transform parent, bool enable )
    {
        while (gm.uncoveredCards.Count > 0)
        {
            stackA.Peek().EnableRaycast(enable);
            stackA.Peek().turnCard(enable);
            stackA.Peek().canBePicked = enable;
            stackA.Peek().transform.SetParent(parent);

            stackB.Add(stackA.Peek());
            stackA.Pop();

            first = null;
            second = null;
            third = null;
        }
        for (int i = gm.record.Count-1; i >= 0; i--) 
        {
            if(gm.record[i].currentCard.isInStock)
            {
                gm.record.RemoveAt(i);
            }
        }
        gm.SubScore(100); //not sure if it was 100 or all the score to this point
        stockReset++;
        yield return new WaitForSeconds(0.5f);
        yield return canClick = true;
    }
    IEnumerator SetUncoveredCardsDraw3(List<Card> listA, bool enable)
    {
        if (listA.Count > 2)
        {
            for (int i = 0; i < 3; i++)
            {
                CardPositioning(listA, enable);               
            }
        }
        else
        {
            for (int i = 0; i < listA.Count; i++)
            {
                CardPositioning(listA, enable);            
            }
        }
        yield return new WaitForSeconds(0.5f);
        yield return canClick = true;
    }
    IEnumerator SetUncoveredCards(List<Card> listA, bool enable)
    {
        CardPositioning(listA,enable);
        yield return new WaitForSeconds(0.5f); 
        yield return canClick = true;
    }
    IEnumerator movingCards(Card card, Transform from, Transform to, float timeOfTravel, int index)
    {
        float normalizedValue;
        float currentTime = 0;
        RectTransform rectTransform = card.GetComponent<RectTransform>();
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel;
            rectTransform.position = Vector3.Lerp(from.position, to.position, normalizedValue);
            yield return null;             
        }
       
        card.transform.SetParent(gm.uncoveredCardsPos[index]);
        yield return null;
    }
  
    void CardPositioning(List<Card> listA, bool enable)
    {
        //record movements
        gm.numCardMoved++;
        CardRecord newRecord = new CardRecord(listA[listA.Count - 1], gm.stockPosition.transform, gm.numCardMoved, gm.GetScore(), listA.Count - 1, stockReset);
        gm.record.Add(newRecord);

        // set the card to be interactable
        listA[listA.Count - 1].EnableRaycast(enable);
        listA[listA.Count - 1].turnCard(enable);
        listA[listA.Count - 1].canBePicked = enable;

        // move the uncovered cards to let only 3 visible 
        RewindUncoveredCards(listA[listA.Count - 1]);
        gm.uncoveredCards.Push(listA[listA.Count - 1]);
        listA.RemoveAt(listA.Count - 1);
    }

    public void RearrangeUncoveredCards()
    { //rerrange uncovered cards to show the last 3 
        uncoveredCardsRearranged = false;
        if (gm.uncoveredCards.Count > 2)
        {
            second.transform.SetParent(gm.uncoveredCardsPos[2]);
            third = second;
 
            first.transform.SetParent(gm.uncoveredCardsPos[1]);
            second = first;

            first = gm.uncoveredCardsPos[0].GetChild(gm.uncoveredCardsPos[0].childCount - 1).GetComponent<Card>();
        }
        uncoveredCardsRearranged = true;
    }
    public void RewindUncoveredCards(Card card)
    {
        uncoveredCardsRearranged = false;
        if (!gm.GetDrawMode())
        {
            if (gm.uncoveredCards.Count == 0)
            {
                first = card;
                StartCoroutine(movingCards(card, card.transform, gm.uncoveredCardsPos[0], timeOfTravel, 0)); 
            }
            else if (gm.uncoveredCards.Count == 1)
            {
                gm.uncoveredCards.Peek().canBePicked = false;                  //if there are already uncovered cards then disable pickup on the last one
                second = card;
                StartCoroutine(movingCards(card, card.transform, gm.uncoveredCardsPos[1], timeOfTravel, 1));
            }
            else if (gm.uncoveredCards.Count == 2)
            {
                gm.uncoveredCards.Peek().canBePicked = false;              //if there are already uncovered cards then disable pickup on the last one
                third = card;
                StartCoroutine(movingCards(card, card.transform, gm.uncoveredCardsPos[2], timeOfTravel, 2));
            }
            else if (gm.uncoveredCards.Count > 2)
            {
                gm.uncoveredCards.Peek().canBePicked = false;                      //if there are already uncovered cards then disable pickup on the last one
                second.transform.SetParent(gm.uncoveredCardsPos[0]);
                third.transform.SetParent(gm.uncoveredCardsPos[1]);
                first = second;                                         //update the first,second and third card reference
                second = third;
                third = card;
                StartCoroutine(movingCards(card, card.transform, gm.uncoveredCardsPos[2], timeOfTravel, 2));
            }
        }
        else if (gm.GetDrawMode()) //removed moving animation in draw3 mode, they were causing problems with rewind
        {
            if (gm.uncoveredCards.Count == 0)
            {
                first = card;
                card.transform.SetParent(gm.uncoveredCardsPos[0]);
            }
            else if (gm.uncoveredCards.Count == 1)
            {
                gm.uncoveredCards.Peek().canBePicked = false;                  //if there are already uncovered cards then disable pickup on the last one
                second = card;
                card.transform.SetParent(gm.uncoveredCardsPos[1]);
            }
            else if (gm.uncoveredCards.Count == 2)
            {
                gm.uncoveredCards.Peek().canBePicked = false;              //if there are already uncovered cards then disable pickup on the last one
                third = card;
                card.transform.SetParent(gm.uncoveredCardsPos[2]);
            }
            else if (gm.uncoveredCards.Count > 2)
            {
                gm.uncoveredCards.Peek().canBePicked = false;                      //if there are already uncovered cards then disable pickup on the last one
                second.transform.SetParent(gm.uncoveredCardsPos[0]);
                third.transform.SetParent(gm.uncoveredCardsPos[1]);
                first = second;                                         //update the first,second and third card reference
                second = third;
                third = card;
                card.transform.SetParent(gm.uncoveredCardsPos[2]);
            }
           
        }
        uncoveredCardsRearranged = true;

    }
  
    
}

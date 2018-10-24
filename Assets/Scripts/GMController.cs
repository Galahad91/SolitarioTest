using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GMController : MonoBehaviour
{
    public CardData data;                                     // reference to the data needed to create a deck
    public Canvas canvas;                                     // reference to canvas
    public Transform background;                              // reference to background component that is parent of all the gameobjects
    public Text scoreText;                                    // reference to the score text
    public Text moves;                                        // reference to the move count text
    public GameObject drawOption;                             // reference to the draw option gameobj
    public Image undoButton;                                  // reference to the undo button
    public Image optionsbutton;                               // reference to the options button  
    public Image exitButton;                                  // reference to the exit button 
    public Transform columnParent;                            // reference to the parent of the 7 columns
    public Transform stacks;                                  // reference to the parent of the 4 stacks
    public StockHandler stockPosition;                        // position of the initial stock
    public Transform[] uncoveredCardsPos;                     // position of the cards removed from the initial stock 
    public Transform[] columnPlacement;                       // position of the first card in each column 

    [HideInInspector] public Stack<Card> movingCards;         // temporary stack used for moving cards
    [HideInInspector] public List<Card> stock;                // initial stack where the shuffled card are stuffed
    [HideInInspector] public Stack<Card> uncoveredCards;      // stack of cards removed from the initial stock
    [HideInInspector] public Card[] deck;                     // list of all cards on the tabletop
    [HideInInspector] public ColumnList[] columns;            // list of all columns script
    [HideInInspector] public StackList[] stackList;           // list of all suit stacks
    [HideInInspector] public List<CardRecord> record;         // list of all legit moves

    private int totalMoves = 0;                               // total moves made until this point
    private int totalCardNumber = 52;                         // total number of cards 
    private bool preparationDone = false;                     // check if the game is ready to be played
    private int totalScore = 0;                               // total score value
    public static bool draw3;                                 // change the draw mode 
    public bool currentDraw;                                  // variable for checking draw mode changes
    private bool canRewind = true;                            // indicates if the rewind CR is done and can rewind again

    [HideInInspector]
    public int numCardMoved = 0;                              // number of cards moved in 1 hand
    public float travelSpeed;                                 // changes the speed of the card distribution at start
     
    public int scoreStandard = 5;
    public int scoreStack = 10;

    void Awake ()
    { 
        record = new List<CardRecord>();
        movingCards = new Stack<Card>();
        stock = new List<Card>();
        uncoveredCards = new Stack<Card>();

        if(draw3)
        {
            drawOption.SetActive(true);
        }
        else
        {
            drawOption.SetActive(false);
        }
        // take all the columns on the tabletop
        columns = new ColumnList[columnParent.childCount];
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i] = columnParent.GetChild(i).GetComponent<ColumnList>();
        }

        // take all the stacks on the tabletop
        stackList = new StackList[stacks.childCount];
        for (int i = 0; i < stacks.childCount; i++)
        {
            stackList[i] = stacks.GetChild(i).GetComponent<StackList>();
        }
      
	}

    private void Start()
    {
        // generate the deck
        DeckGeneration();
        // shuffle the cards and puts them in the stock list
        StockShuffle();
        // distribute the card in each column
        StartCoroutine(CardDistribution());
    
    }


    #region Methods
    // Start preparations
    void SetColor(Card card)
    {
        if (card.GetSuit() == SUIT.Spades || card.GetSuit() == SUIT.Clubs)         
            card.SetCardColor(COLOR.Black);        
        else
            card.SetCardColor(COLOR.Red);
    }
    void DeckGeneration()
    {
        int currentCount = 0; //current position in the deck
        deck = new Card[totalCardNumber];

        for (int i = 0; i < data.Suits.Length; i++)     //number of suits
        {
            for (int y = 0; y < totalCardNumber/ data.Suits.Length; y++)     //number of card per suit
            {
                //Instantiate a new card prefab and add it to the deck array
                deck[currentCount] = Instantiate(data.card,stockPosition.transform.position,Quaternion.identity,stockPosition.transform).GetComponent<Card>();
                deck[currentCount].suitImage.sprite = data.Suits[i];
                deck[currentCount].num.sprite = data.Num[y];
                deck[currentCount].image.sprite = data.Suits[i];
               
                //Setting color, suit and number variables in card script
                if (i == 0)
                {
                    deck[currentCount].SetSuit(SUIT.Hearts);                   
                }
                else if (i == 1)
                {
                    deck[currentCount].SetSuit(SUIT.Diamonds);
                }
                else if(i == 2)
                {
                    deck[currentCount].SetSuit(SUIT.Clubs);
                }
                else if(i == 3)
                {
                    deck[currentCount].SetSuit(SUIT.Spades);
                }

                SetColor(deck[currentCount]);
                deck[currentCount].SetNumber( y + 1);
                currentCount++;

            }

        }
    }
    void StockShuffle()
    {
        while(stock.Count < deck.Length)
        {
            int r = Random.Range(0, deck.Length);
            if (!deck[r].isInStock)
            {
                stock.Add(deck[r]);
                deck[r].isInStock = true;
            }
        }

    }
    IEnumerator CardDistribution()
    {
        //get the total number of cards needed for the distribution in columns
        yield return new WaitForSeconds(0.1f); 
        for (int i = 0; i < columns.Length; i++)
        {
            for (int y = 0; y < columns[i].cardAtStart; y++)
            {                                                                  
                Transform currentCard = stock[stock.Count - 1].transform; 
                Card card = currentCard.GetComponent<Card>();

                currentCard.SetParent(background); 
                stock.RemoveAt(stock.Count-1);
                card.isInStock = false;

                StartCoroutine(MoveCards(currentCard, i, card, y));


                yield return new WaitForSeconds(0.1f);
            }

        }  
        // the game is ready to be played
        preparationDone = true;
        yield return null;
    }
    IEnumerator MoveCards(Transform currentCard,int i, Card card, int y)
    {
        //add distribution animation here 
        float timeOfTravel = travelSpeed; 
        float normalizedValue;
        float currentTime = 0;
        RectTransform rectTransform = currentCard.GetComponent<RectTransform>();
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel;
            rectTransform.position = Vector3.Lerp(currentCard.transform.position, columnPlacement[i].transform.position, normalizedValue);           
            yield return null;  
        }

        currentCard.SetParent(columns[i].transform);
        columns[i].cardInColumn.Push(card);

        //if it's the last card of the column enable raycast target and turn animation
        if (y + 1 == columns[i].cardAtStart)
        {
            card.turnCard(true);
            card.EnableRaycast(true);
            card.canBePicked = true;
        }
 
        yield return null;
    }
    
    // Options   
    public bool GetDrawMode()
    {
        return draw3;
    }
    public void SetDrawMode(bool enable)
    {
        draw3 = enable;
    }

    // gameplay utility
    public void UpdateCurrentNewColumn(Card card, ColumnList thisColumn)
    {
        // update parent and add the current card to this column
        card.drag.currentParent = thisColumn.transform;
        thisColumn.cardInColumn.Push(card);

    }
    public void RearrangeOldColumn(Card card)
    {
        ColumnList currentColumn = card.drag.currentParent.GetComponent<ColumnList>();
        // enable turn and drag on the last card of the old column
        if (currentColumn != null)
        {
            currentColumn.cardInColumn.Pop();
            if (currentColumn.cardInColumn.Count > 0)
            {
                currentColumn.cardInColumn.Peek().turnCard(true);
                currentColumn.cardInColumn.Peek().canBePicked = true;
                AddScore(scoreStandard);//Add score
            }
        }
        else if (currentColumn == null) // if the column component doesn't exist then the card must come from the uncovered list
        {
            uncoveredCards.Peek().isInStock = false;
            uncoveredCards.Pop();
            stockPosition.RearrangeUncoveredCards();
            AddScore(scoreStandard);  //Add score
            if (uncoveredCards.Count > 0)
            {
                uncoveredCards.Peek().canBePicked = true;
            }
        }
    }
    public void AddMoves() 
    {
        totalMoves++;
        moves.text = totalMoves.ToString();
    }
    public bool GetCanRewind()
    {
        return canRewind;
    }
    public void AddScore(int score)
    {
        totalScore += score;
        scoreText.text = totalScore.ToString();
    }
    public void SubScore(int score)
    {
        totalScore -= score;
        if (totalScore < 0)
            totalScore = 0;
        scoreText.text = totalScore.ToString();
        AddMoves();
    }
    public int GetScore()
    {
        return totalScore;
    }
    public bool GetPrepDone()
    {
        return preparationDone;
    }
    public IEnumerator Rewind()
    {
        canRewind = false;
        if (record.Count > 0)
        {
            int stepsBack = record[record.Count-1].numCardsInMove;
            for (int i = 0; i < stepsBack; i++)
            {
                StackList currentStack = record[record.Count - 1].currentCard.transform.parent.GetComponent<StackList>();
                ColumnList currentColumn = record[record.Count - 1].currentCard.transform.parent.GetComponent<ColumnList>();
                ColumnList columnParent = record[record.Count - 1].originalParent.GetComponent<ColumnList>();
                //**Find from which stack came and put it back there**
                //the parent was a column
                if (columnParent != null)
                {
                    if (columnParent.cardInColumn.Count > 0)
                    {
                        if (record[record.Count - 1].prevCardCovered)
                        {
                            columnParent.cardInColumn.Peek().turnCard(false);
                            columnParent.cardInColumn.Peek().canBePicked = false;
                        }
                        else
                        {
                            columnParent.cardInColumn.Peek().turnCard(true);
                            columnParent.cardInColumn.Peek().canBePicked = true;
                        }
                    }
                    columnParent.cardInColumn.Push(record[record.Count - 1].currentCard);
                    //sub score
                }
                //the parent was the uncovered stack
                else if (record[record.Count - 1].wasInStock && !record[record.Count - 1].wasCovered && record[record.Count - 1].couldBePicked)
                {
                    stockPosition.RewindUncoveredCards(record[record.Count - 1].currentCard);
                    record[record.Count - 1].originalParent = record[record.Count - 1].currentCard.transform.parent; 
                    uncoveredCards.Push(record[record.Count - 1].currentCard);
                }
                //the parent was the stock
                else if (record[record.Count - 1].wasInStock && record[record.Count - 1].wasCovered && !record[record.Count - 1].couldBePicked)
                {
                    stock.Insert(record[record.Count - 1].stockIndex, record[record.Count - 1].currentCard); 
                    if (draw3)
                    {
                        if (record[record.Count - 1].stockReset != stockPosition.stockReset)
                        {
                            stepsBack = 1; // avoid overflow in the list after the stack resets
                        }
                    }
                }


                //**Find in which stack is right now and remove it from there before the rewind**
                if (currentColumn != null)
                {
                    currentColumn.cardInColumn.Pop();
                }
                //the parent is the uncovered stack
                else if (record[record.Count - 1].currentCard.isInStock && !record[record.Count - 1].currentCard.GetIsCovered() && record[record.Count - 1].currentCard.canBePicked)
                {                    
                    uncoveredCards.Pop();
                    if (uncoveredCards.Count > 0)
                    {   //once removed the current card activate the last one in the stack
                        uncoveredCards.Peek().canBePicked = true;
                    }
                    stockPosition.RearrangeUncoveredCards(); //rearrange uncovered cards
                }
                //the parent is one of the suit stacks
                else if (currentStack != null)
                {
                    currentStack.cardInStack.Pop();
                }

                //reset old parent
                record[record.Count - 1].currentCard.transform.SetParent(record[record.Count - 1].originalParent);
                //reset covered state
                if (record[record.Count - 1].wasCovered != record[record.Count - 1].currentCard.GetIsCovered())
                {
                    if (record[record.Count - 1].wasCovered)
                        record[record.Count - 1].currentCard.turnCard(false);
                    else
                        record[record.Count - 1].currentCard.turnCard(true);
                }

                //reset other stats
                record[record.Count - 1].currentCard.canBePicked = record[record.Count - 1].couldBePicked;
                record[record.Count - 1].currentCard.isInStock = record[record.Count - 1].wasInStock;
                record[record.Count - 1].currentCard.EnableRaycast(record[record.Count - 1].raycastEnabled);
                SubScore(totalScore - record[record.Count - 1].score); 
                record.RemoveAt(record.Count - 1);
            }
            yield return new WaitForSeconds(0.5f);
        }
        canRewind = true;      
    }
     #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // parameters
    private SUIT suit;
    private COLOR color;
    private int number;

    private bool hasColorBeenSet = false;
    private bool hasSuitBeenSet = false;
    private bool hasNumberBeenSet = false;

    private bool covered = true;              //check if the card is covered 
    // components
    public Image num;
    public Image image;
    public Image suitImage;
    public Animator anim;
    [HideInInspector] public Drag drag;
    [HideInInspector] public Image backImage;

    public bool isInStock;                  //check if the card is already in the stock, needed for the shuffle method at start
    public bool canBePicked = false;        //check if the card can be picked by the player

    private void Awake()
    {
        backImage = GetComponent<Image>(); 
        drag = GetComponent<Drag>();
    }

    public void SetNumber(int num)
    {
        if (!hasNumberBeenSet)
        {
            number = num;
            hasNumberBeenSet = true;
        }
        else
            Debug.Log("already set");
    }
    public int GetNumber()
    {
        return number;
    }

    public void SetSuit(SUIT s)
    {
        if (!hasSuitBeenSet)
        {
            suit = s;
            hasSuitBeenSet = true;
        }
        else
            Debug.Log("already set");
    }
    public SUIT GetSuit()
    {
        return suit;
    }

    public void SetCardColor(COLOR c)
    {
        if (!hasColorBeenSet)
        {
            color = c;
            hasColorBeenSet = true;
        }
        else
            Debug.Log("already set");
    }
    public COLOR GetCardColor()
    {
        return color;
    }

    public bool GetIsCovered()
    {
        return covered;
    }
    public void EnableRaycast(bool enable)
   {
        backImage.raycastTarget = enable;
   }
    public void turnCard(bool canTurn)
    {
        if(canTurn)
        {
            anim.SetBool("canFlip",true);
            covered = false;
        }
        else
        {
            anim.SetBool("canFlip", false);
            covered = true;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOption : MonoBehaviour, IPointerClickHandler
{
    public GameObject confirm;
    GMController gm;
    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GMController>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(confirm.activeSelf)
        {
            confirm.SetActive(false);
            gm.SetDrawMode(false); 
        }
        else
        {
            confirm.SetActive(true);
            gm.SetDrawMode(true); 
        }
    }
}

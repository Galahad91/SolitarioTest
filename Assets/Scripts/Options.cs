using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Options : MonoBehaviour, IPointerClickHandler
{
    public GameObject pausePanel;
    GMController gm;
    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GMController>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(gm.GetCanRewind() && gm.GetPrepDone())
        {
            gm.currentDraw = gm.GetDrawMode();
            pausePanel.SetActive(true); 
        }
    }
}

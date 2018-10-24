using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Undo : MonoBehaviour, IPointerClickHandler
{
    GMController gm;
    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GMController>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(gm.GetCanRewind() && gm.GetPrepDone())
        {
            StartCoroutine(gm.Rewind());
        }
    }
}

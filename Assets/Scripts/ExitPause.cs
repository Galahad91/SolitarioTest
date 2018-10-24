using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ExitPause : MonoBehaviour, IPointerClickHandler 
{
    public GameObject pausePanel;
    GMController gm;
    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GMController>();
    } 
    public void OnPointerClick(PointerEventData eventData)
    {
        if(gm.currentDraw != gm.GetDrawMode())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name );
        }
        else
        {
            pausePanel.SetActive(false); 
        }
    }
}

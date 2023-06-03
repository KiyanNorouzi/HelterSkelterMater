using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    private Animator anim;
    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1;
        anim = pauseMenuPanel.GetComponent<Animator>();
        anim.enabled = false;
    }



    public void PauseGame()
    {
        anim.enabled = true;
        anim.Play("PauseMenuSlideIn");      
        isPaused = true;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        anim.Play("PauseMenuSlideOut");
        Time.timeScale = 1;
    }

}
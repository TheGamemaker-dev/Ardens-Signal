using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    GameObject pausePanel;

    bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                Pause();
            }
            else
            {
                Unpuase();
            }
        }
    }

    public void Unpuase()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        pausePanel.transform.SetAsLastSibling();
        Time.timeScale = 0;
    }
}

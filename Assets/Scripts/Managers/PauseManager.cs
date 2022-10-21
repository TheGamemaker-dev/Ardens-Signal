using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    GameObject pausePanel;

    [SerializeField]
    Button saveButton,
        quitButton;

    GameManager manager;

    bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        manager = GameManager.singleton;
        saveButton.onClick.AddListener(manager.Save);
        quitButton.onClick.AddListener(manager.Quit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
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
        isPaused = false;
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        pausePanel.transform.SetAsLastSibling();
        Time.timeScale = 0;
        isPaused = true;
    }
}

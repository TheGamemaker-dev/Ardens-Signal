using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    GameObject pausePanel;

    bool isPaused = false;

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

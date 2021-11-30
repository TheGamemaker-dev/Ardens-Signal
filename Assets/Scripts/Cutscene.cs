using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    public void CutsceneDone()
    {
        gameObject.SetActive(false);
    }
    public void PlaySound(string soundName)
    {
        FindObjectOfType<AudioManager>().PlaySound(soundName);
    }
    public void SetFlag(string flag)
    {
        GameManager.SetFlag(flag);
    }
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}

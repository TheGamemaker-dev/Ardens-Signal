using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Houses all of the functions called by the cutscene animators
/// </summary>
public class Cutscene : MonoBehaviour
{
    public void CutsceneDone()
    {
        gameObject.SetActive(false);
    }

    public void PlaySound(string soundName)
    {
        FindObjectOfType<AudioManager>().PlaySound(soundName, false);
    }

    public void SetFlag(string flag)
    {
        GameManager.singleton.SetFlag(flag);
    }

    public void ChangeScene(string scene)
    {
        GameManager.singleton.ChangeScene(scene);
    }
}

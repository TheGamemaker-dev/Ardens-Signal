using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

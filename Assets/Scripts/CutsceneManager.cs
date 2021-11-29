using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    Cutscene[] cutsceneParents;
    void Awake()
    {
        cutsceneParents = FindObjectsOfType<Cutscene>();
    }
    public void StartCutscene(int day)
    {
        GameObject cutsceneParent = cutsceneParents.FirstOrDefault(x => x.gameObject.name.RemoveWhitespace() == "Day" + day.ToString()).gameObject;
        cutsceneParent.SetActive(true);
        Animator animator = cutsceneParent.GetComponent<Animator>();
        animator.SetTrigger("fire");
    }
}

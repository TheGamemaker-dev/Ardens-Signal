using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneManager : MonoBehaviour
{
    Cutscene[] cutsceneParents;
    public UnityAction cutsceneStarted;
    void Awake()
    {
        cutsceneParents = FindObjectsOfType<Cutscene>();
    }
    void OnEnable()
    {
        GameManager.onFlagSet += OnEndDay;
    }
    void OnDisable()
    {
        GameManager.onFlagSet -= OnEndDay;
    }
    public void StartCutscene(int day)
    {
        if (day >= 4) return;
        cutsceneStarted?.Invoke();
        transform.SetAsLastSibling();
        GameObject cutsceneParent = cutsceneParents.FirstOrDefault(x => x.gameObject.name.RemoveWhitespace() == "Day" + day.ToString()).gameObject;
        cutsceneParent.SetActive(true);
        Animator animator = cutsceneParent.GetComponent<Animator>();
        animator.SetTrigger("fire");
    }
    void OnEndDay(string dayDone)
    {
        bool hasProperSyntax = dayDone.Remove(3) == "day" && dayDone.Substring(4) == "Done";
        bool hasNum = int.TryParse(dayDone[3].ToString(), out int day);

        if (hasProperSyntax && hasNum)
        {
            StartCutscene(day + 1);
        }
    }
}

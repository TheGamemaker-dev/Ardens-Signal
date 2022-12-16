using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public UnityAction cutsceneStarted;

    [SerializeField]
    [Tooltip("Put these cutscenes in the order they appear in the game")]
    VideoClip[] cutscenes; //in chronological order

    VideoPlayer player;
    Image coverPanel;

    void Awake()
    {
        player = GetComponent<VideoPlayer>();
        coverPanel = GetComponent<Image>();
    }

    void OnEnable()
    {
        GameManager.onFlagSet += OnEndDay;
    }

    void OnDisable()
    {
        GameManager.onFlagSet -= OnEndDay;
    }

    public IEnumerator StartCutscene(int day)
    {
        Debug.Log(day);
        coverPanel.enabled = false;
        Fade fade = FindObjectOfType<Fade>();
        if (day != 1)
        {
            cutsceneStarted?.Invoke();
            fade.FadeOut();
            yield return new WaitForSeconds(fade.length + 2);
        }
        if (day >= 3) //Nothing past day 3 yet
        {
            GameManager.singleton.ChangeScene("Demo");
            yield break;
        }
        player.clip = cutscenes[day - 1];
        transform.SetAsLastSibling();
        player.Play();
        if (day == 1)
        {
            yield return new WaitForSeconds(13);
            AudioManager.singleton.PlaySound("Fan Whirring", false);
            yield return new WaitForSeconds(((float)player.clip.length - 13));
        }
        else
        {
            yield return new WaitForSeconds(((float)player.clip.length));
            GameManager.SetFlag("day" + day.ToString() + "Start");
            fade.FadeIn();
        }
        transform.SetAsFirstSibling();
        coverPanel.enabled = true;
    }

    void OnEndDay(string dayDone)
    {
        bool hasProperSyntax = dayDone.Remove(3) == "day" && dayDone.Substring(4) == "Done";
        bool hasNum = int.TryParse(dayDone[3].ToString(), out int day);
        bool triggeredYet = GameManager.GetFlagState("day" + (day + 1) + "Start");

        if (hasProperSyntax && hasNum && !triggeredYet)
        {
            StartCoroutine(StartCutscene(day + 1));
        }
    }
}

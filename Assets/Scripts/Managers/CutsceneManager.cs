using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public UnityAction cutsceneStarted;

    [SerializeField] VideoClip[] cutscenes; //in chronological order

    VideoPlayer player;
    void Awake()
    {
        player = GetComponent<VideoPlayer>();
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
        Fade fade = FindObjectOfType<Fade>();
        if (day != 1)
        {
            cutsceneStarted?.Invoke();
            fade.FadeOut();
            yield return new WaitForSeconds(fade.length + 2);
        }
        if (day >= 3)
        {
            GameManager.singleton.ChangeScene("Demo");
            yield break;
        }
        player.clip = cutscenes[day - 1];
        transform.SetAsLastSibling();
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
    }
    void OnEndDay(string dayDone)
    {
        bool hasProperSyntax = dayDone.Remove(3) == "day" && dayDone.Substring(4) == "Done";
        bool hasNum = int.TryParse(dayDone[3].ToString(), out int day);

        if (hasProperSyntax && hasNum)
        {
            StartCoroutine(StartCutscene(day + 1));
        }
    }
}

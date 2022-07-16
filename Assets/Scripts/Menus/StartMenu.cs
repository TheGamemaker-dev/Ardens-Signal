using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    InputField inputField;

    [SerializeField]
    Button submit;

    AudioManager manager;

    void Start()
    {
        UpdateButton("");
        manager = FindObjectOfType<AudioManager>();
        manager.PlaySound("Dream", true);
    }

    public void UpdateButton(string value)
    {
        submit.interactable = value.RemoveWhitespace() != "";
    }

    public void StartGame()
    {
        StartCoroutine(DoStartGame());
    }

    public void StartNewGame()
    {
        GameManager.singleton.playerName = inputField.text;
        GameManager.singleton.isNewGame = true;
        StartGame();
    }

    IEnumerator DoStartGame()
    {
        Fade fade = FindObjectOfType<Fade>();
        fade.FadeOut();
        yield return new WaitForSeconds(fade.length + 2);
        GameManager.singleton.ChangeScene("Game");
    }

    public void StartGame()
    {
        StartCoroutine(DoStartGame());
    }

    IEnumerator DoStartGame()
    {
        Fade fade = FindObjectOfType<Fade>();
        fade.FadeOut();
        yield return new WaitForSeconds(fade.length + 2);
        GameManager.singleton.ChangeScene("Game");
    }
}

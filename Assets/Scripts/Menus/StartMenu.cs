using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    InputField inputField;

    [SerializeField]
    Button submit,
        continueButton;

    AudioManager manager;

    void Start()
    {
        UpdateButton("");
        manager = FindObjectOfType<AudioManager>();
        manager.PlaySound("Dream", true);
        continueButton.interactable = File.Exists(GameManager.saveFile);
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
        File.Delete(GameManager.saveFile);
        StartGame();
    }

    IEnumerator DoStartGame()
    {
        Fade fade = FindObjectOfType<Fade>();
        fade.FadeOut();
        yield return new WaitForSeconds(fade.length + 2);
        GameManager.singleton.ChangeScene("Game");
    }
}

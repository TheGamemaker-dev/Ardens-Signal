using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Button submit;

    AudioManager manager;

    void Start()
    {
        UpdateButton("");
        manager = FindObjectOfType<AudioManager>();
        manager.PlaySound("Dream");
    }

    public void UpdateButton(string value)
    {
        submit.interactable = value.RemoveWhitespace() != "";
    }

    public void SetName()
    {
        GameManager.singleton.playerName = inputField.text;
    }
}

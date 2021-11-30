using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{

    [SerializeField] InputField passwordField;
    [SerializeField] Text nameText, errorText;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = GameManager.singleton.playerName;
    }

    public void LogIn()
    {
        if (passwordField.text == "password")
        {
            GameManager.SetFlag("logIn");
            Destroy(gameObject);
        }
        else
        {
            errorText.text = "Password is incorrect";
        }
    }
}

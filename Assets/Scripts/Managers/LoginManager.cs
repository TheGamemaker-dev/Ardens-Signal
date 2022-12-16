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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LogIn();
        }
    }

    public void LogIn()
    {
        if (passwordField.text == "password")
        {
            Invoke(nameof(SetLogInFlag), 2);
            FindObjectOfType<AudioManager>().PlaySound("Accomplishment", false);
            gameObject.transform.SetAsFirstSibling();
        }
        else
        {
            errorText.text = "Password is incorrect";
        }
    }

    void SetLogInFlag()
    {
        GameManager.SetFlag("logIn");
        Destroy(gameObject);
    }
}

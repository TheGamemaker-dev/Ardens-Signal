using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice
{
    string choiceMessage;
    string jumpTo;

    public Choice(string choiceMessage, string jumpTo)
    {
        this.choiceMessage = choiceMessage;
        this.jumpTo = jumpTo;
    }
}

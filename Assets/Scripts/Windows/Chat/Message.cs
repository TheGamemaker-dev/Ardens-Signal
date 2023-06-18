using System;
using UnityEngine.Events;

public class Message : IEquatable<Message>
{
    public string message { get; set; }
    public Choice[] choices { get; set; }
    public string setFlag { get; set; }
    public string playSound { get; set; }
    public bool fromYou { get; set; }

    public Message()
    {
        message = "";
        choices = new Choice[] { };
        fromYou = false;
        playSound = "";
        setFlag = "";
    }

    public bool Equals(Message other)
    {
        if (other == null)
        {
            return false;
        }
        bool output = true;

        output &= message == other.message;
        output &= choices.IsEqualTo(other.choices);

        return output;
    }

    public Message NextMessage()
    {
        if (choices.Length == 1)
        {
            return choices[0].jumpTo;
        }
        else
        {
            throw new Exception("Message has more than one choice");
        }
    }

    public Message NextMessage(int choice)
    {
        return choices[choice].jumpTo;
    }
}

public class Message
{
    public string message { get; set; }
    public Choice[] choices { get; set; }
    public string jumpTo { get; set; }

    public Message()
    {
        message = "";
        choices = new Choice[] { };
        jumpTo = "";
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
        output &= jumpTo == other.jumpTo;

        return output;
    }
}

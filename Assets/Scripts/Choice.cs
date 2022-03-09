public class Choice
{
    public string choiceMessage { get; }
    public string jumpTo { get; }

    public Choice(string choiceMessage, string jumpTo)
    {
        this.choiceMessage = choiceMessage;
        this.jumpTo = jumpTo;
    }
}

using System.Collections.Generic;

public class MessageGroup
{
    public enum InstructionTypes { FromHeader, FlagsHeader, SetFlag, Stop };

    public string[] fullFile { get; }
    public Dictionary<int, string> jumps { get; }
    public Dictionary<int, Message> messages { get; }
    public Dictionary<int, string> instructions { get; }
    public Dictionary<int, string> lineTypes { get; }
    public string from { get; }
    public string[] flagsRequired { get; }

    public MessageGroup(string[] fullFile, Dictionary<int, string> jumps, Dictionary<int, Message> messages, Dictionary<int, string> instructions, Dictionary<int, string> lineTypes, string from, string[] flagsRequired)
    {
        this.fullFile = fullFile;
        this.jumps = jumps;
        this.messages = messages;
        this.instructions = instructions;
        this.lineTypes = lineTypes;
        this.from = from;
        this.flagsRequired = flagsRequired;
    }
}

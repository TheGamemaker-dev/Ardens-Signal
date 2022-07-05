using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public static class MessageGroupCompiler
{
    public static MessageGroup Compile(TextAsset file)
    {
        string[] fullFile = file.text.Split(
            new char[] { '\n' },
            System.StringSplitOptions.RemoveEmptyEntries
        );
        Dictionary<int, string> jumps = new Dictionary<int, string>();
        Dictionary<int, Message> messages = new Dictionary<int, Message>();
        Dictionary<int, string> instructions = new Dictionary<int, string>();
        Dictionary<int, string> lineTypes = new Dictionary<int, string>();
        string from = "";
        string[] flagsRequired = new string[] { };

        for (int i = 0; i < fullFile.Length; i++)
        {
            string line = fullFile[i];

            //if it starts with \t (tab), then its a message or response
            if (line[0] == '\t')
            {
                if (line[1] != '\t')
                {
                    Message message = new Message();
                    int iChange = 0;
                    //check for choices
                    if (line[line.Length - 2] == '=') //a message with a = at the end has choices
                    {
                        List<Choice> choices = new List<Choice>();
                        line = line.Remove(line.Length - 1);
                        int lineChangeCheck = 1;
                        while (fullFile[i + lineChangeCheck][2] == '+') //third character of a choice line is always a +
                        {
                            string choiceLine = fullFile[i + lineChangeCheck].Substring(3);
                            string[] choiceParams = choiceLine.Split('|');
                            Choice choice;

                            if (choiceParams.Length == 1)
                            {
                                choice = new Choice(choiceParams[0].RemoveTabs(), "");
                            }
                            else
                            {
                                choice = new Choice(
                                    choiceParams[0].RemoveTabs(),
                                    choiceParams[1].RemoveLineBreaks()
                                );
                            }
                            choices.Add(choice);
                            lineTypes.Add(i + lineChangeCheck, "choice");
                            lineChangeCheck++;
                        }
                        message.choices = choices.ToArray();
                        iChange = lineChangeCheck - 1;
                    }

                    //check for jump, a message will never have both a jump and choices
                    string[] messageParams = line.Split('|');
                    if (messageParams[0] != line)
                    {
                        message.message = messageParams[0].RemoveTabs();
                        message.jumpTo = messageParams[1].RemoveLineBreaks();
                    }
                    else
                    {
                        message.message = new Regex(@"=+")
                            .Replace(line.RemoveTabs(), "")
                            .RemoveLineBreaks();
                    }
                    string messageWithDVar = message.message;
                    //modify message for dyanmic variables
                    for (int j = 0; j < message.message.Length; j++)
                    {
                        if (message.message[j] == '{')
                        {
                            int k = 1;
                            string variable = "";
                            while (message.message[j + k] != '}')
                            {
                                variable += message.message[j + k];
                                k++;
                            }
                            FieldInfo varInfo = GameManager.singleton.GetType().GetField(variable);
                            string varValue = varInfo.GetValue(GameManager.singleton).ToString();

                            messageWithDVar = messageWithDVar.Replace(
                                "{" + variable + "}",
                                varValue
                            );
                        }
                    }
                    message.message = messageWithDVar;

                    messages.Add(i, message);
                    lineTypes.Add(i, "message");
                    i += iChange;
                }
            }
            else if (line[0] == '~') //it's an instruction
            {
                string[] instructionsParams = line.Substring(1).Split(':');
                switch (instructionsParams[0].RemoveLineBreaks())
                {
                    case "FROM":
                        from = instructionsParams[1].RemoveLineBreaks();
                        break;
                    case "FLAGSREQUIRED":
                        string[] flags = instructionsParams[1].Split(',');
                        for (int j = 0; j < flags.Length; j++)
                        {
                            flags[j] = flags[j].RemoveLineBreaks();
                        }
                        flagsRequired = flags;
                        break;
                    case "SETFLAG":
                    case "STOP":
                    case "PLAYSOUND":
                        break;
                    default:
                        throw new UnityException(
                            "Instruction not set properly: " + instructionsParams[0]
                        );
                }
                instructions.Add(i, line.RemoveLineBreaks().Substring(1));
                lineTypes.Add(i, "instruction");
            }
            else //its a jump
            {
                string jump = line.Remove(line.Length - 2);
                jumps.Add(i, jump);
                lineTypes.Add(i, "jump");
            }
        }

        MessageGroup output = new MessageGroup(
            jumps,
            messages,
            instructions,
            lineTypes,
            from,
            flagsRequired
        );
        return output;
    }
}

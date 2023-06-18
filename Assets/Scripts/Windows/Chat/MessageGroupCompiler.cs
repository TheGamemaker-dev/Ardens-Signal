using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Reflection;

public static class MessageGroupCompiler
{
    public static MessageGroup Compile(TextAsset file)
    {
        string[] fullFile = file.text.Split(
            new char[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );
        string from = "";
        List<string> flagsRequired = new List<string>();
        Message rootMessage = new Message();

        int lineNum = 0;
        while (fullFile[lineNum][0] == '~') //Preprocess instructions
        {
            string line = fullFile[lineNum].Substring(1);
            string[] args = line.Split(':');
            switch (args[0])
            {
                case "FROM":
                    if (from != "")
                    {
                        Debug.LogError("FROM already set in " + file.name);
                    }
                    from = args[1];
                    break;
                case "FLAGSREQUIRED":
                    flagsRequired.Add(args[1]);
                    break;
                case "SETFLAG":
                case "PLAYSOUND":
                    Debug.LogError(
                        "Should not be encountering " + args[0] + " at this point in " + file.name
                    );
                    break;
                default:
                    Debug.LogError("Unknown instruction " + args[0] + " in " + file.name);
                    break;
            }
            lineNum++;
        }

        while (fullFile[lineNum][0] != '\t') //Skip blank lines
        {
            lineNum++;
        }

        rootMessage = BuildMessageTree(fullFile, lineNum);

        MessageGroup output = new MessageGroup(
            from,
            flagsRequired.ToArray(),
            file.name,
            rootMessage
        );

        return output;
    }

    static Message BuildMessageTree(string[] file, int lineNumber)
    {
        //Base case
        if (lineNumber >= file.Length)
        {
            return null;
        }

        Message output = new Message();
        List<Choice> choices = new List<Choice>();

        //Get line
        string line = file[lineNumber];

        if (line[0] == '~') //Check if its an instruction
        {
            line = line.Substring(1);
            string[] args = line.Split(':');
            switch (args[0])
            {
                case "SETFLAG":
                    output.setFlag = args[1];
                    break;
                case "PLAYSOUND":
                    output.playSound = args[1];
                    break;
                case "FROM":
                case "FLAGSREQUIRED":
                    throw new Exception(
                        "Should not be encountering " + args[0] + " at this point in the file"
                    );
                default:
                    throw new Exception(
                        "Unknown instruction " + args[0] + " in " + file[lineNumber]
                    );
            }
            lineNumber++;
            line = file[lineNumber];
        }

        if (line[0] == '\t') //Check if its a message
        {
            line = line.Substring(1);
            line = ReplaceVariables(line);

            if (line[0] == '_')
            {
                output.fromYou = true;
                line = line.Substring(1);
            }

            if (line[line.Length - 1] == '=') //Check for choices
            {
                line = line.Substring(0, line.Length - 1);
                output.message = line;
                int choiceLineNum = lineNumber + 1;
                string choiceLine = file[choiceLineNum].Substring(2);
                while (choiceLine[0] == '+')
                {
                    choiceLine = choiceLine.Substring(1);
                    Choice choice = new Choice();
                    bool hasJump = choiceLine.Contains("|");
                    if (hasJump)
                    {
                        string[] jumpArgs = choiceLine.Split('|');
                        choice.choiceMessage = jumpArgs[0];
                        int jumpLine = FindJump(file, jumpArgs[1]);

                        if (jumpLine == -1)
                        {
                            throw new Exception("Could not find jump target " + jumpArgs[1]);
                        }

                        choice.jumpTo = BuildMessageTree(file, jumpLine + 1);
                        choices.Add(choice);
                    }
                    else
                    {
                        choice.choiceMessage = choiceLine;
                        int nextMessageLine = choiceLineNum + 1;
                        while (file[nextMessageLine][1] == '\t')
                        {
                            nextMessageLine++;
                        }
                        choice.jumpTo = BuildMessageTree(file, nextMessageLine);
                        choices.Add(choice);
                    }
                    choiceLineNum++;
                    choiceLine =
                        choiceLineNum < file.Length ? file[choiceLineNum].Substring(2) : null;
                }
            }
            else if (line.Contains("|"))
            {
                string[] args = line.Split('|');
                Choice choice = new Choice();
                output.message = args[0];
                int jumpLine = FindJump(file, args[1]);
                if (jumpLine == -1)
                {
                    throw new Exception("Could not find jump target " + args[1]);
                }
                choice.jumpTo = BuildMessageTree(file, jumpLine + 1);
                choices.Add(choice);
            }
            else
            {
                output.message = line;
                Choice choice = new Choice();
                choice.jumpTo = BuildMessageTree(file, lineNumber + 1);
                choices.Add(choice);
            }
        }

        output.choices = choices.ToArray();
        return output;
    }

    static int FindJump(string[] file, string jumpTarget)
    {
        string pattern = jumpTarget + ":";

        for (int i = 0; i < file.Length; i++)
        {
            if (Regex.IsMatch(file[i], pattern))
            {
                return i;
            }
        }
        return -1;
    }

    static string ReplaceVariables(string input)
    {
        if (!input.Contains("{"))
        {
            return input;
        }

        string output = "";
        int start = input.IndexOf('{');
        int end = input.IndexOf('}');
        string variable = input.Substring(start + 1, end - start - 1);

        output += input.Substring(0, start);
        output += GameManager.singleton.GetVariable(variable);
        output += input.Substring(end + 1);

        return output;
    }
}

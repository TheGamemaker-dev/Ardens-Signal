using System.Text.RegularExpressions;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public static class ExtentionMethods
{
    private static readonly Regex sTabs = new Regex(@"\t+");
    private static readonly Regex sLinebreaks = new Regex(@"[\r\n]+");
    private static readonly Regex sWhitespace = new Regex(@"\s+");

    public static string RemoveTabs(this string input)
    {
        return sTabs.Replace(input, "");
    }

    public static string RemoveLineBreaks(this string input)
    {
        return sLinebreaks.Replace(input, "");
    }

    public static string RemoveWhitespace(this string input)
    {
        return sWhitespace.Replace(input, "");
    }

    public static Vector2Int Round(this Vector2 input)
    {
        int x = Mathf.RoundToInt(input.x);
        int y = Mathf.RoundToInt(input.y);

        Vector2Int output = new Vector2Int(x, y);
        return output;
    }

    public static bool IsEqualTo<T>(this IEnumerable<T> input1, IEnumerable<T> input2)
    {
        return input1.Count() == input2.Count() && !input1.Except(input2).Any();
    }
}

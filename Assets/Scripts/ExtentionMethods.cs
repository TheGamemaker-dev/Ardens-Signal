using System.Text.RegularExpressions;

public static class ExtentionMethods
{
    private static readonly Regex sTabs = new Regex(@"\t+");
    private static readonly Regex sWhitespace = new Regex(@"\s+");
    public static string RemoveTabs(this string input)
    {
        return sTabs.Replace(input, "");
    }
    public static string RemoveLineBreaks(this string input)
    {
        return sWhitespace.Replace(input, "");
    }
}

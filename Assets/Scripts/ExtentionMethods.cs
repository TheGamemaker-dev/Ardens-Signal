using System.Text.RegularExpressions;

public static class ExtentionMethods
{
    private static readonly Regex sTabs = new Regex(@"\t+");
    private static readonly Regex sLinebreaks = new Regex(@"[\r\n]+");
    public static string RemoveTabs(this string input)
    {
        return sTabs.Replace(input, "");
    }
    public static string RemoveLineBreaks(this string input)
    {
        return sLinebreaks.Replace(input, "");
    }
}

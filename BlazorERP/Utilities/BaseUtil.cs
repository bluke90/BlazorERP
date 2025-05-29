namespace BlazorERP.Utilities;

public static class BaseUtil
{
    
    public static int SumList<T>(this List<T> list, Func<T, int> selector)
    {
        // This method calculates the sum of a list of items based on a selector function.
        if (list == null || list.Count == 0)
            return 0;

        int sum = 0;
        foreach (var item in list)
        {
            sum += selector(item);
        }
        return sum;
    }
    
    public static string Pluralize(this string word)
    {
        if (word.Length == 1)
            return word;
        
        // Simple pluralization logic, can be extended for more complex rules.
        if (word.EndsWith("y") && !word.EndsWith("ay") && !word.EndsWith("ey") && !word.EndsWith("oy") && !word.EndsWith("uy"))
            return word.Substring(0, word.Length - 1) + "ies";
        
        if (word.EndsWith("s") || word.EndsWith("x") || word.EndsWith("z") || word.EndsWith("ch") || word.EndsWith("sh"))
            return word + "es";
        
        return word + "s";
    }
    
}
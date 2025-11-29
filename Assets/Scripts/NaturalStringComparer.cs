using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DefaultNamespace
{
    public class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;

            // Split the string into text and numbers
            // Example: "Stage 10" becomes ["Stage ", "10"]
            string[] xParts = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
            string[] yParts = Regex.Split(y.Replace(" ", ""), "([0-9]+)");

            for (int i = 0; i < xParts.Length && i < yParts.Length; i++)
            {
                if (xParts[i] != yParts[i])
                {
                    // Try to convert both parts to numbers
                    if (int.TryParse(xParts[i], out int xNum) && int.TryParse(yParts[i], out int yNum))
                    {
                        // If both are numbers, compare numerically (2 vs 10)
                        return xNum.CompareTo(yNum);
                    }
                
                    // If not numbers, compare alphabetically ("a" vs "b")
                    return xParts[i].CompareTo(yParts[i]);
                }
            }

            // If we reach here, one string is a prefix of the other
            return xParts.Length.CompareTo(yParts.Length);
        }
    }
}
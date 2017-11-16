using System;
using System.Collections.Generic;
using System.Text;

namespace InternalsVisibleToFixer.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitAtUpperCase(this string input)
        {
            if (input.Length <= 1)
            {
                yield return input;
                yield break;
            }

            var currentSubstring = new StringBuilder();
            currentSubstring.Append(input[0]);
            for (var i = 1; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]))
                {
                    yield return currentSubstring.ToString();
                    currentSubstring.Clear();
                }
                currentSubstring.Append(input[i]);
            }
            yield return currentSubstring.ToString();
        }  
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InternalsVisibleToFixer.Extensions;

namespace InternalsVisibleToFixer.DistanceCalculation
{
    internal class CamelCaseLevensteinDistanceCalculator : IStringDistanceCalculator
    {
        private const int Penalty = 5;
        private static readonly IStringDistanceCalculator Levenstein = new LevensteinDistanceCalculator();

        public int CalculateDistance(string first, string second)
        {
            var tokens = first.SplitAtUpperCase().ToArray();
            var regexString = GenerateRegexString(tokens);
            var match = new Regex(regexString,RegexOptions.Compiled)
                .Match(second);
            
            if (match.Success)
            {
                return CalculateInsertionsNeededToChangeFirstToSecond(match);
            }
            return Levenstein.CalculateDistance(first, second) * Penalty;
        }

        private static int CalculateInsertionsNeededToChangeFirstToSecond(Match match)
        {
            var result = match.Groups[0].Index;
            for (var i = 1; i < match.Groups.Count -1; i++)
            {
                var previousGroup = match.Groups[i - 1];
                result += match.Groups[i].Index - (previousGroup.Index + previousGroup.Length);
            }
            /* currently we ignore distance from last match to end of string.
             * Penalty should be hight enough, so this error does not count.
             */

            return result;
        }

        private static string GenerateRegexString(IReadOnlyList<string> tokens)
        {
            var result = new StringBuilder(tokens.Count*10);
            result.Append("^.*");
            for (var index = 0; index < tokens.Count; index++)
            {
                var token = tokens[index];
                result.Append($"(?<t{index}>{token}.*)");
            }
            result.Append(".*$");

            return result.ToString();
        }
    }
}
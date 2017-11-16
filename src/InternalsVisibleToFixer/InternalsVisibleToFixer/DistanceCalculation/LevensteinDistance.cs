using System;

namespace InternalsVisibleToFixer.DistanceCalculation
{
    internal class LevensteinDistanceCalculator : IStringDistanceCalculator
    {
        public int CalculateDistance(string first, string second)
        {
            //adapted from: https://www.dotnetperls.com/levenshtein
            var fstLength = first.Length;
            var sndLength = second.Length;
            var distances = new int[fstLength + 1, sndLength + 1];

            if (fstLength == 0) return sndLength;
            if (sndLength == 0) return fstLength;

            for (var i = 0; i <= fstLength; distances[i, 0] = i++){ }

            for (var j = 0; j <= sndLength; distances[0, j] = j++){}

            for (var i = 1; i <= fstLength; i++)
            {
                for (var j = 1; j <= sndLength; j++)
                {
                    var cost = second[j - 1] == first[i - 1] ? 0 : 1;

                    distances[i, j] = Math.Min(
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost);
                }
            }
            return distances[fstLength, sndLength];
        }
    }
}
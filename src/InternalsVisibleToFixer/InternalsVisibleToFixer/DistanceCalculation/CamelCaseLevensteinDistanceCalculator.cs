namespace InternalsVisibleToFixer.DistanceCalculation
{
    internal class CamelCaseLevensteinDistanceCalculator : IStringDistanceCalculator
    {
        private static IStringDistanceCalculator _levenstein = new LevensteinDistanceCalculator();

        public int CalculateDistance(string first, string second)
        {
            throw new System.NotImplementedException();
        }
    }
}
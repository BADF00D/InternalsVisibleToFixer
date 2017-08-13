using System.Collections.Generic;
using InternalsVisibleToFixer.DistanceCalculation;
using NUnit.Framework;

namespace InternalsVisibleToFixer.Test.DistanceCalculations.LevensteinDistanceCalculatorSpecs
{
    public class LevensteinDistanceCalculatorTests
    {
        private readonly LevensteinDistanceCalculator _distancecalculator = new LevensteinDistanceCalculator();

        [Test, TestCaseSource(nameof(TestCases))]
        public int Should_be_correct(string first, string second)
        {
            return _distancecalculator.CalculateDistance(first, second);
        }


        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData("abc", "abc").Returns(0);
                yield return new TestCaseData("xyz", "abc").Returns(3);
                yield return new TestCaseData("Tier", "Tor").Returns(2);
                yield return new TestCaseData("live", "life").Returns(1);
                yield return new TestCaseData("animal", "animla").Returns(2);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using InternalsVisibleToFixer.DistanceCalculation;
using NUnit.Framework;

namespace InternalsVisibleToFixer.Test.DistanceCalculations.CamelCaseLevensteinDistanceCalculatorSpecs
{
    [TestFixture]
    public class If_running_CamelCaseDistanceCalculator
    {
        private readonly IStringDistanceCalculator _sut = new CamelCaseLevensteinDistanceCalculator(); 

        [TestCaseSource(nameof(TestCases))]
        public string The_most_suitable_item_should_be_returned(string query, string[] source)
        {
            return source
                .Select(item => Tuple.Create(_sut.CalculateDistance(query, item), item))
                .OrderBy(tpl => tpl.Item1)
                .Select(tpl => tpl.Item2)
                .FirstOrDefault();
        }

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return SearchIn("SomeAssembly", "FirstAssembly", "Short", "AnotherAssembly")
                    .Querying("Some")
                    .Returns("SomeAssembly");

                yield return SearchIn("SomeAssembly", "FirstAssembly", "Short", "AnotherAssembly")
                    .Querying("FA")
                    .Returns("FirstAssembly");

                yield return SearchIn("SomeAssembly", "FirstAssembly", "Short", "AnotherAssembly")
                    .Querying("aA")
                    .Returns("Short");

                yield return SearchIn("SomeAssembly", "FirstAssembly", "Short", "AnotherAssembly")
                    .Querying("irst")
                    .Returns("FirstAssembly");
            }
        }

        private static string[] SearchIn(params string[] source)
        {
            return source;
        }
    }

    internal static class TesTCaseDataExtensions
    {
        public static TestCaseData Querying(this string[] source, string query)
        {
            return new TestCaseData(query, source);
        }

        public static TestCaseData NamedCorrectly(this TestCaseData testCase)
        {
            var query = testCase.Arguments[0] as string ?? "error";
            var source = testCase.Arguments[1] as string[] ?? new[] {"error"};
            
            var name = $"'{query}' in '{string.Join(",", source)}' yields '{testCase.Result}'";

            return testCase.SetName(name);
        }
    }
}
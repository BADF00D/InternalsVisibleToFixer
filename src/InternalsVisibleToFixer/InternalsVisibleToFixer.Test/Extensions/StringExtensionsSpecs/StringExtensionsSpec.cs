using System.Collections.Generic;
using System.Linq;
using InternalsVisibleToFixer.Extensions;
using NUnit.Framework;

namespace InternalsVisibleToFixer.Test.Extensions.StringExtensionsSpecs
{
    [TestFixture]
    internal class StringExtensionsSpec
    {
        [Test, TestCaseSource(nameof(TestCases))]
        public string[] Should_result_be_ok(string source)
        {
            return source.SplitAtUpperCase().ToArray();
        }

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData("IVT")
                    .ReturnsArrayWith("I", "V", "T")
                    .SetName("IVT - ['I','V','T']");
                yield return new TestCaseData("mhhThisIsAwesome")
                    .ReturnsArrayWith("mhh", "This", "Is", "Awesome")
                    .SetName("mhhThisIsAwesome - ['mhh','This','Is', 'Awesome']"); 
                yield return new TestCaseData("only lower case")
                    .ReturnsArrayWith("only lower case")
                    .SetName("only lower case - ['only lower case']");
                yield return new TestCaseData("")
                    .ReturnsArrayWith("")
                    .SetName("<string.empty>");
            }
        } 
         
    }

    internal static class TestCaseDataExtension
    {
        public static TestCaseData ReturnsArrayWith<T>(this TestCaseData source, params T[] items)
        {
            return source.Returns(items);
        }
    }
}
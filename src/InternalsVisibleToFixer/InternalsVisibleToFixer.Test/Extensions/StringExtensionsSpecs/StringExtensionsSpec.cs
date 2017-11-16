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

        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData("IVT")
                    .Returns("I", "V", "T");
                yield return new TestCaseData("mhhThisIsAwesome")
                    .Returns("mhh", "This", "Is", "Awesome");
                yield return new TestCaseData("only lower case")
                    .Returns("only lower case");
                yield return new TestCaseData("")
                    .Returns("");
            }
        } 
         
    }

    internal static class TestCaseDataExtension
    {
        public static TestCaseData Returns<T>(this TestCaseData source, params T[] items)
        {
            return source.Returns(items);
        }
    }
}
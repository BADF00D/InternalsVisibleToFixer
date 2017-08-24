using NUnit.Framework;

namespace InternalsVisibleToFixer.Test.Issue
{
    [TestFixture]
    internal abstract class IssueSpec : Spec
    {
        protected readonly InternalsVisibleToAnalyzer Sut;

        protected IssueSpec()
        {
            Sut = new InternalsVisibleToAnalyzer();
        }
    }
}
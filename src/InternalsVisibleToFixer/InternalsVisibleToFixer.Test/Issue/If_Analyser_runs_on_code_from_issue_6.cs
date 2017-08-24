using NUnit.Framework;

namespace InternalsVisibleToFixer.Test.Issue
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_6 : IssueSpec
    {
        private const string Code = @"[assembly: InternalsVisibleTo(";

        protected override void BecauseOf()
        {
            MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Exception()
        {
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using InternalsVisibleToFixer.Configuration;
using InternalsVisibleToFixer.DistanceCalculation;
using InternalsVisibleToFixer.ProposalGeneration;
using NUnit.Framework;

namespace InternalsVisibleToFixer.Test.ProposalGeneration.ProposalGeneratorSpecs
{
    [TestFixture]
    internal class If_ProposalGenerator_is_called : Spec
    {
        private readonly ProposalGenerator _sut;

        private const string CurrentProject = "SomeProjectName";
        private const string CurrentToken = "Extern3";

        private readonly string[] _otherProjects = {"OtherProject1", "OtherProject2", "OtherProject3", "OtherProject4"};
        private readonly string[] _existingReferences = {"OtherProject1", "OtherProject4", "ExternalProject1"};
        private readonly string[] _externalReferences = {"ExternalProject1", "ExternalProject2", "ExternalProject3"};
        private string[] _result;
        private readonly IConfiguration _configuration = A.Fake<IConfiguration>(o => o.Strict());
        private const int MaxNumberOfProposal = 3;

        public If_ProposalGenerator_is_called()
        {
            A.CallTo(() => _configuration.MaxNumberOfProposalToShow).Returns(MaxNumberOfProposal);
            A.CallTo(() => _configuration.ExternalReferences).Returns(_externalReferences);

            _sut = new ProposalGenerator(new LevensteinDistanceCalculator(), _configuration);
        }

        protected override void BecauseOf()
        {
            _result = _sut.Generate(CurrentToken, CurrentProject, _existingReferences, _otherProjects).ToArray();
        }

        [Test]
        public void Then_CurrentProject_should_not_be_part_of_proposal()
        {
            _result.Should().NotContain(CurrentProject);
        }

        [Test]
        public void Then_there_should_be_no_more_then_3_proposals()
        {
            _result.Count().Should().BeLessOrEqualTo(MaxNumberOfProposal);
        } 

        [Test]
        public void Then_proposal_should_be_in_correct_order()
        {
            _result.Should().ContainInOrder("ExternalProject3", "ExternalProject2", "OtherProject2");
        }

        [Test]
        public void Then_propsal_should_not_contain_existing_proposals()
        {
            _result.Should().NotContain(_existingReferences);
        }
    }
}
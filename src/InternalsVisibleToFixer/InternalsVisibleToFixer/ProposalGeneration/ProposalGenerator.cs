using System.Collections.Generic;
using System.Linq;
using InternalsVisibleToFixer.Configuration;
using InternalsVisibleToFixer.DistanceCalculation;
using InternalsVisibleToFixer.Extensions;

namespace InternalsVisibleToFixer.ProposalGeneration
{
    internal class ProposalGenerator : IProposalGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly IStringDistanceCalculator _distanceCalculator;

        public ProposalGenerator(IStringDistanceCalculator distanceCalculator, IConfiguration configuration)
            //todo when configuration is changeable, this has to be updateable
        {
            _distanceCalculator = distanceCalculator;
            _configuration = configuration;
        }

        public IEnumerable<string> Generate(string currentToken, string currentProject,
            IReadOnlyList<string> existingReferences, IReadOnlyList<string> otherProjects)
        {
            return _configuration.ExternalReferences
                .Concat(otherProjects)
                .Except(existingReferences)
                .Except(currentProject)
                .Take(_configuration.MaxNumberOfProposalToShow)
                .OrderBy(reference => _distanceCalculator.CalculateDistance(currentToken, reference));
        }
    }
}
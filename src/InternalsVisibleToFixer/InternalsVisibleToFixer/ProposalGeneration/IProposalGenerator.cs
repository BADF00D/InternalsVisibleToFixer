using System.Collections.Generic;

namespace InternalsVisibleToFixer.ProposalGeneration
{
    internal interface IProposalGenerator
    {
        IEnumerable<string> Generate(
            string currentToken,
            string currentProject,
            IReadOnlyList<string> existingReferences,
            IReadOnlyList<string> otherProjects);

    }
}
using System.Collections.Generic;

namespace InternalsVisibleToFixer.Configuration
{
    public interface IConfiguration
    {
        /// <summary>
        /// Allowed external references.
        /// </summary>
        IReadOnlyList<string> ExternalReferences { get; }
        
        /// <summary>
        /// Max Number of project references to show.
        /// </summary>
        int MaxNumberOfProposalToShow { get; }
    }
}
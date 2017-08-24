using System.Collections.Generic;

namespace InternalsVisibleToFixer.Configuration
{
    internal class DefaultConfiguration : IConfiguration
    {
        public DefaultConfiguration()
        {
            ExternalReferences = new[]
            {
                "DynamicProxyGenAssembly2"
            };
            MaxNumberOfProposalToShow = 7;
        }

        public IReadOnlyList<string> ExternalReferences { get; }
        public int MaxNumberOfProposalToShow { get; }
    }
}
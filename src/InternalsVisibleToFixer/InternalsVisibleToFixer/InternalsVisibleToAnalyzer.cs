using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using InternalsVisibleToFixer.Configuration;
using InternalsVisibleToFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace InternalsVisibleToFixer {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InternalsVisibleToAnalyzer : DiagnosticAnalyzer {
        public const string UnknownReferenceId = "InternalsVisibleToFixer_UnknownProjectReference";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Wrong usage";
        


        private static readonly DiagnosticDescriptor UnknownReferenceRule = new DiagnosticDescriptor(UnknownReferenceId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(UnknownReferenceRule);

        public override void Initialize(AnalysisContext context) {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.Attribute);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context) {
            var attribute = context.Node as AttributeSyntax;

            if (!attribute.IsInternalsVisibleToAttribute()) return;

            var referencedProject = attribute.GetReferencedProjectOrDefault();
            
            var solution = context.GetSolution();

            var projectNames = solution.Projects.Select(p => p.AssemblyName);
            var allowedFriendAssemblies = ConfigurationManager.Instance.ExternalReferences
                .Concat(projectNames)
                .ToArray();

            if (allowedFriendAssemblies.Contains(referencedProject)) return;

            var properties = ImmutableDictionary<string, string>.Empty
                .Add(Constants.CurrentInternalsVisibleToTokenContent, referencedProject)
                //.Add(Constants.ProjectsOfSolution, string.Join(Constants.ValueSeperator, projectNames))
                //.Add(Constants.AdditionalNonSolutionReferences, string.Join(Constants.ValueSeperator, AllowedFriendAssemblies))
                ;
            context.ReportDiagnostic(Diagnostic.Create(UnknownReferenceRule, attribute.GetLocation(), properties));
        }
    }
}

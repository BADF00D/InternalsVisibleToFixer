using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using InternalsVisibleToFixer.Configuration;
using InternalsVisibleToFixer.DistanceCalculation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using InternalsVisibleToFixer.Extensions;
using InternalsVisibleToFixer.ProposalGeneration;

namespace InternalsVisibleToFixer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InternalsVisibleToCodeFixProvider)), Shared]
    public class InternalsVisibleToCodeFixProvider : CodeFixProvider
    {
        private const string TitleFormat = "Replace {0} with {1}";
        private static readonly IStringDistanceCalculator DistanceCalculator = new CamelCaseLevensteinDistanceCalculator();

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(InternalsVisibleToAnalyzer.UnknownReferenceId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            
            var attribute = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AttributeSyntax>().First();
            var properties = context.Diagnostics.First().Properties;
            var currentToken = properties[Constants.CurrentInternalsVisibleToTokenContent];
            var refrencesAlreadyMade = GetProjectsAlreadyReferencedByOtherInternalsVisibleToAttributes(root, currentToken);

            var projectsOfSolution = context.Document.Project.Solution
                .Projects
                .Select(p => p.Name)
                .ToArray();

            var proposalGenerator = new ProposalGenerator(DistanceCalculator, ConfigurationManager.Instance);
            var suggestedReferences = proposalGenerator.Generate(currentToken ?? string.Empty, context.Document.Project.Name,
                refrencesAlreadyMade, projectsOfSolution);
            foreach (var project in suggestedReferences)
            {
                var title = string.Format(TitleFormat, currentToken, project);
                context.RegisterCodeFix(
                    CodeAction.Create(title, c => ReplaceReference(attribute, project, context), title), diagnostic);
            }
        }

        private static string[] GetProjectsAlreadyReferencedByOtherInternalsVisibleToAttributes(SyntaxNode root, string currentToken)
        {
            return root
                .DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(@as => @as.IsInternalsVisibleToAttribute())
                .Select(@as => @as.GetReferencedProjectOrDefault())
                .Where(reference => reference != null)
                .Except(new[] { currentToken })
                .ToArray();
        }

        private static async Task<Document> ReplaceReference(SyntaxNode attribute, string project,
            CodeFixContext context)
        {
            var newAttribute = SyntaxFactory.Attribute(
                SyntaxFactory.IdentifierName("InternalsVisibleTo"))
                .WithArgumentList(
                    SyntaxFactory.AttributeArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.AttributeArgument(
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal(project))))));


            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            var newRoot = root.ReplaceNode(attribute, newAttribute);

            return context.Document.WithSyntaxRoot(newRoot);
        }
    }
}
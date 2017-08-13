using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace InternalsVisibleToFixer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InternalsVisibleToCodeFixProvider)), Shared]
    public class InternalsVisibleToCodeFixProvider : CodeFixProvider
    {
        private const string TitleFormat = "Replace {0} with {1}";

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

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var attribute =
                root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AttributeSyntax>().First();
            var properties = context.Diagnostics.First().Properties;
            var currentToken = properties[Constants.CurrentInternalsVisibleToTokenContent];
            var projectsOfSolution = properties[Constants.ProjectsOfSolution]
                .Split(new[] {Constants.ValueSeperator}, StringSplitOptions.RemoveEmptyEntries);
            var additionalAllowedReferences = properties[Constants.AdditionalNonSolutionReferences]
                .Split(new[] {Constants.ValueSeperator}, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var project in projectsOfSolution.Except(new [] {context.Document.Project.Name}))
            {
                var title = string.Format(TitleFormat, currentToken, project);
                context.RegisterCodeFix(
                    CodeAction.Create(title, c => ReplaceReference(attribute, project, context), title), diagnostic);
            }
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
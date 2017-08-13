using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace InternalsVisibleToFixer.Extensions
{
    public static class AttributeSyntaxExtensions
    {
        private const string InternalsVisibleTo = "InternalsVisibleTo";

        public static bool IsInternalsVisibleToAttribute(this AttributeSyntax attribute)
        {
            return attribute?
                .DescendantNodes<IdentifierNameSyntax>()
                .FirstOrDefault(ins => ins.Identifier.Text == InternalsVisibleTo) 
                != null;
        }
        

        public static string GetReferencedProjectOrDefault(this AttributeSyntax attribute)
        {
            var argument = attribute.DescendantNodes().OfType<AttributeArgumentSyntax>().FirstOrDefault();
            var literalExpression = argument?.Expression as LiteralExpressionSyntax;
            return literalExpression?.Token.ValueText;
        }
    }
}

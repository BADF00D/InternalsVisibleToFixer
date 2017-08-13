using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace InternalsVisibleToFixer.Extensions
{
    internal static class SyntaxNodeExtensions
    {
        public static IEnumerable<T> DescendantNodes<T>(this SyntaxNode node) where T : SyntaxNode
        {
            return node.DescendantNodes().OfType<T>();
        }
    }
}

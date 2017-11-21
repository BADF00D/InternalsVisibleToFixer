using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace InternalsVisibleToFixer.Extensions
{
    internal static class RoslynExtensions
    {
        public static Solution GetSolution(this SyntaxNodeAnalysisContext context)
        {
            var workspaceobj = context.Options.GetPrivatePropertyValue<object>("Workspace");
            if (workspaceobj != null)
            {
                return workspaceobj.GetPrivatePropertyValue<Solution>("CurrentSolution");
            }

            var tyy = context.Options.GetType().GetField("_solution", BindingFlags.NonPublic | BindingFlags.Instance);
            var solution = tyy.GetValue(context.Options) as Solution;
            return solution;

        }

        private static T GetPrivatePropertyValue<T>(this object obj, string propName) where T : class{
            if (obj == null) {
                return null;
            }

            var pi = obj.GetType().GetRuntimeProperties()
                .FirstOrDefault(p => p.Name == propName);


            if (pi == null) {
                return null;
            }

            return (T)pi.GetValue(obj, null);
        }
    }
}
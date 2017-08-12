﻿using System;
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

            return workspaceobj.GetPrivatePropertyValue<Solution>("CurrentSolution");
        }

        private static T GetPrivatePropertyValue<T>(this object obj, string propName) {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            var pi = obj.GetType().GetRuntimeProperties()
                .FirstOrDefault(p => p.Name == propName);


            if (pi == null) {
                throw new ArgumentOutOfRangeException(nameof(propName),
                    $"Property {propName} was not found in Type {obj.GetType().FullName}");
            }

            return (T)pi.GetValue(obj, null);
        }
    }
}
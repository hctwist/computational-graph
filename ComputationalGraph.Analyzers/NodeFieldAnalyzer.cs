using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ComputationalGraph.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NodeFieldAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        "CGRAPH0001",
        Resources.CGRAPH0001Title,
        Resources.CGRAPH0001MessageFormat,
        "NodeFields",
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [Rule];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSyntax, SymbolKind.Field);
    }

    private static void AnalyzeSyntax(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IFieldSymbol field)
        {
            return;
        }

        if (field.Type is not INamedTypeSymbol fieldType)
        {
            return;
        }

        if (!fieldType.IsGenericType)
        {
            return;
        }
        
        INamedTypeSymbol nodeType = context.Compilation.GetTypeByMetadataName("ComputationalGraph.Core.Node`1")?.ConstructUnboundGenericType() ??
            throw new InvalidOperationException("Could not find node type");
        
        if (SymbolEqualityComparer.Default.Equals(fieldType.ConstructUnboundGenericType(), nodeType))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, field.Locations.Single()));
        }
    }
}
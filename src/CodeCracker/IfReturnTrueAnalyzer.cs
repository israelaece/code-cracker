using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CodeCracker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfReturnTrueAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CC0007";
        internal const string Title = "Return Condition directly";
        internal const string Message = "{0}";
        internal const string Category = "Syntax";
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyzer, SyntaxKind.IfStatement);
        }

        private void Analyzer(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = context.Node as IfStatementSyntax;
            if (ifStatement?.Else == null) return;
            var statementInsideIf = ifStatement.Statement.GetSingleStatementFromPossibleBlock();
            if (statementInsideIf == null) return;
            var statementInsideElse = ifStatement.Else.Statement.GetSingleStatementFromPossibleBlock();
            if (statementInsideElse == null) return;
            var returnIf = statementInsideIf as ReturnStatementSyntax;
            var returnElse = statementInsideElse as ReturnStatementSyntax;
            if (returnIf == null || returnElse == null) return;
            if ((returnIf.Expression is LiteralExpressionSyntax && returnIf.Expression.IsKind(SyntaxKind.TrueLiteralExpression) &&
                returnElse.Expression is LiteralExpressionSyntax && returnElse.Expression.IsKind(SyntaxKind.FalseLiteralExpression)) ||
                (returnIf.Expression is LiteralExpressionSyntax && returnIf.Expression.IsKind(SyntaxKind.FalseLiteralExpression) &&
                returnElse.Expression is LiteralExpressionSyntax && returnElse.Expression.IsKind(SyntaxKind.TrueLiteralExpression)))
            {
                var diagnostic = Diagnostic.Create(Rule, ifStatement.IfKeyword.GetLocation(), "You should return directly.");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
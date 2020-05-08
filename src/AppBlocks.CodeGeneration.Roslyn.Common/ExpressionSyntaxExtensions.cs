using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppBlocks.CodeGeneration.Roslyn.Common
{
	internal static class ExpressionSyntaxExtensions
	{
		public static ExpressionSyntax Parenthesize(
			this ExpressionSyntax expression, bool includeElasticTrivia = true)
		{
			// a 'ref' expression should never be parenthesized.  It fundamentally breaks the code.
			// This is because, from the language's perspective there is no such thing as a ref
			// expression.  instead, there are constructs like ```return ref expr``` or 
			// ```x ? ref expr1 : ref expr2```, or ```ref int a = ref expr``` in these cases, the 
			// ref's do not belong to the exprs, but instead belong to the parent construct. i.e.
			// ```return ref``` or ``` ? ref  ... : ref ... ``` or ``` ... = ref ...```.  For 
			// parsing convenience, and to prevent having to update all these constructs, we settled
			// on a ref-expression node.  But this node isn't a true expression that be operated
			// on like with everything else.
			if (expression.IsKind(SyntaxKind.RefExpression))
			{
				return expression;
			}

			return ParenthesizeWorker(expression, includeElasticTrivia);
		}

		private static ExpressionSyntax ParenthesizeWorker(
			this ExpressionSyntax expression, bool includeElasticTrivia)
		{
			var withoutTrivia = expression.WithoutTrivia();
			var parenthesized = includeElasticTrivia
				? SyntaxFactory.ParenthesizedExpression(withoutTrivia)
				: SyntaxFactory.ParenthesizedExpression(
					SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.OpenParenToken, SyntaxTriviaList.Empty),
					withoutTrivia,
					SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.CloseParenToken, SyntaxTriviaList.Empty));

			return parenthesized.WithTriviaFrom(expression);
		}
	}
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagUnitTestFromTestlist.Categorizer
{
    public class DocumentManipulator
    {
        Document _document;
        Workspace _workspace;

        public DocumentManipulator(Document document, Workspace workspace)
        {
            _workspace = workspace;
            _document = document;
        }

        public SyntaxTree CategorizeTestMethods(List<string> testmethods, string category)
        {
            var existingSyntaxNode = GetSyntaxNodeFromDocument().Result;
            SyntaxTree newSyntaxTree = SyntaxFactory.SyntaxTree(existingSyntaxNode);

            var originalMethods = existingSyntaxNode
                                    .DescendantNodes()
                                    .OfType<MethodDeclarationSyntax>()
                                    .Where(m => 
                                        IsTestMethod(m) 
                                        && testmethods.Contains(m.Identifier.Value));

            foreach (var originalMethod in originalMethods)
            {
                var methodFromNewSyntaxTree = GetMethodFromSyntaxRoot(newSyntaxTree.GetCompilationUnitRoot(), originalMethod);
                var testCategoryAttribute = FindAttributeWithName(methodFromNewSyntaxTree, "TestCategory");

                if (testCategoryAttribute != null)
                {
                    var isAlreadyTaggetWithCategory = testCategoryAttribute.ArgumentList.Arguments.Any(a => a.ToString().Equals(category));
                    if (isAlreadyTaggetWithCategory)
                    {
                        continue;
                    }
                }

                var newMethod = AddMethodProperty(methodFromNewSyntaxTree, "TestCategory", category);
                var newSyntaxNode = newSyntaxTree.GetRoot().ReplaceNode(methodFromNewSyntaxTree, newMethod);

                newSyntaxTree = SyntaxFactory
                                    .SyntaxTree(Formatter.Format(newSyntaxNode, _workspace))
                                    .WithFilePath(methodFromNewSyntaxTree.SyntaxTree.FilePath);
            }

            return newSyntaxTree;
        }
        
        private bool IsTestMethod(MethodDeclarationSyntax method)
        {
            var isTestMethod = method.AttributeLists
                                .Any(al => al.Attributes
                                            .Any(a => 
                                                a.Name is IdentifierNameSyntax 
                                                && ((IdentifierNameSyntax)a.Name).Identifier.Value.Equals("TestMethod")));

            return isTestMethod;
        }

        private AttributeSyntax FindAttributeWithName(MethodDeclarationSyntax method, string name)
        {
            var attributes = method.AttributeLists.SelectMany(al => al.Attributes).ToList();
            return attributes.SingleOrDefault(a => a.Name is IdentifierNameSyntax && ((IdentifierNameSyntax)a.Name).Identifier.Value.Equals(name));
        }

        private async Task<SyntaxNode> GetSyntaxNodeFromDocument()
        {
            var root = await _document.GetSyntaxRootAsync().ConfigureAwait(false);
            return root;
        }

        static MethodDeclarationSyntax AddMethodProperty(MethodDeclarationSyntax method, string propertyName, string argumentName)
        {
            return method.AddAttributeLists(
                    SyntaxFactory.AttributeList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Attribute(
                                SyntaxFactory.IdentifierName(propertyName),
                                SyntaxFactory.AttributeArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.AttributeArgument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Token(
                                                    default(SyntaxTriviaList),
                                                    SyntaxKind.StringLiteralToken,
                                                    argumentName,
                                                    argumentName,
                                                    default(SyntaxTriviaList))
                                                ))))))));
        }

        static MethodDeclarationSyntax GetMethodFromSyntaxRoot(CompilationUnitSyntax root, MethodDeclarationSyntax method)
        {
            try
            {
                var methodsInSyntaxRoot = root
                                            .DescendantNodes()
                                            .OfType<ClassDeclarationSyntax>()
                                            .Select(b => b.Members.OfType<MethodDeclarationSyntax>()
                                                .SingleOrDefault(m => m.Identifier.ValueText == method.Identifier.ValueText 
                                                                    && m.ParameterList.ToString() == method.ParameterList.ToString()))
                                            .Where(m => m != null);

                return methodsInSyntaxRoot.Single();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

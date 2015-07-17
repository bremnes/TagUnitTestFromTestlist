using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagUnitTestFromTestlist.Categorizer
{
    public class DocumentManipulator
    {
        Document _document;
        Workspace _workspace;
        SyntaxNode _syntaxNode;
        bool _madeChanges = false;
        
        public DocumentManipulator(Document document) : this(document, MSBuildWorkspace.Create())
        {
        }

        public DocumentManipulator(Document document, Workspace workspace)
        {
            _workspace = workspace;
            _document = document;
            _syntaxNode = _document.GetSyntaxRootAsync().Result;
        }

        public DocumentManipulator(SyntaxTree syntaxTree) : this(syntaxTree.GetRoot())
        {
        }

        public DocumentManipulator(SyntaxNode syntaxNode)
        {
            _syntaxNode = syntaxNode;
            _workspace = MSBuildWorkspace.Create();
        }

        public SyntaxNode SyntaxNode { get { return _syntaxNode; } }

        public bool HasRecategorized { get { return _madeChanges; } }

        public void CategorizeTestMethods(List<string> testmethods, string category)
        {
            var methods = _syntaxNode
                                    .DescendantNodes()
                                    .OfType<MethodDeclarationSyntax>()
                                    .Where(m => 
                                        IsTestMethod(m) 
                                        && testmethods.Contains(m.Identifier.Value));

            foreach (var method in methods)
            {
                var testCategoryAttributes = FindAttributesWithName(method, "TestCategory");
                if (testCategoryAttributes.Any(tca => tca.ArgumentList.Arguments.Any(a => a.ToString().Equals(category))))
                {
                    continue;
                }

                var newMethodWithTestCategory = AddMethodAttribute(method, "TestCategory", category);
                var oldMethodFromManipulatedSyntaxNode = GetMethodFromCurrentSyntaxRoot(method);
                _syntaxNode = _syntaxNode.ReplaceNode(oldMethodFromManipulatedSyntaxNode, newMethodWithTestCategory);

                _madeChanges = true;
            }
        }

        public void SaveChanges(string path)
        {
            var syntaxTree = SyntaxFactory.SyntaxTree(Formatter.Format(_syntaxNode, _workspace));

            using (StreamWriter file = File.CreateText(path))
            {
                file.Write(syntaxTree.ToString());
                file.Flush();
            }
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

        private IEnumerable<AttributeSyntax> FindAttributesWithName(MethodDeclarationSyntax method, string name)
        {
            var attributes = method.AttributeLists.SelectMany(al => al.Attributes).ToList();
            return attributes.Where(a => a.Name is IdentifierNameSyntax && ((IdentifierNameSyntax)a.Name).Identifier.Value.Equals(name));
        }

        private static MethodDeclarationSyntax AddMethodAttribute(MethodDeclarationSyntax method, string attributeName, string argumentName)
        {
            return method.AddAttributeLists(
                    SyntaxFactory.AttributeList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Attribute(
                                SyntaxFactory.IdentifierName(attributeName),
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
        
        private MethodDeclarationSyntax GetMethodFromCurrentSyntaxRoot(MethodDeclarationSyntax method)
        {
            try
            {
                var methodsInSyntaxRoot = _syntaxNode
                                            .DescendantNodes()
                                            .OfType<ClassDeclarationSyntax>()
                                            .Select(b => b.Members.OfType<MethodDeclarationSyntax>()
                                                .SingleOrDefault(m => m.Identifier.ValueText == method.Identifier.ValueText
                                                                    && m.ParameterList.ToString() == method.ParameterList.ToString()))
                                            .Where(m => m != null);

                return methodsInSyntaxRoot.Single();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

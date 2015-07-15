using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        public DocumentManipulator(Document document)
        {
            _document = document;
        }

        public Document CategorizeTestMethods()
        {
            var existingSyntaxNode = GetSyntaxNodeFromDocument().Result;
            var methods = existingSyntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>();

            return _document;
        }

        private async Task<SyntaxNode> GetSyntaxNodeFromDocument()
        {
            var root = await _document.GetSyntaxRootAsync().ConfigureAwait(false);
            return root;
        }
    }
}

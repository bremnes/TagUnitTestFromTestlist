using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagUnitTestFromTestlist.Tests.Builders
{
    internal class DocumentBuilder
    {
        internal static Document Build(string name, string code)
        {
            Document document;
            using (var adhocWorkspace = new AdhocWorkspace())
            {
                var solution = adhocWorkspace.CurrentSolution;
                var newProject = adhocWorkspace.AddProject("Project", LanguageNames.CSharp);
                document = adhocWorkspace.AddDocument(newProject.Id, "TestFile.cs", SourceText.From(code));
            }

            return document;
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Linq;

namespace TagUnitTestFromTestlist.Tests
{
    [TestClass]
    public class AdhocWorkspaceTests
    {
        [TestMethod]
        public void ManualTestToCheckLogic()
        {
            var adhocWorkspace = new AdhocWorkspace();
            var solution = adhocWorkspace.CurrentSolution;

            var newProject = adhocWorkspace.AddProject("Project.Test", LanguageNames.CSharp);
            adhocWorkspace.AddDocument(newProject.Id, "TestFile.cs", SourceText.From("public class Bar { }"));

            Assert.AreEqual(1, adhocWorkspace.CurrentSolution.Projects.Count());
            var project = adhocWorkspace.CurrentSolution.Projects.Single();
            Assert.AreEqual("Project.Test", project.Name);
            Assert.AreEqual(1, project.Documents.Count());
            Assert.AreEqual("TestFile.cs", project.Documents.Single().Name);
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TagUnitTestFromTestlist.Categorizer;

namespace TagUnitTestFromTestlist.Tests.Categorizer
{
    [TestClass]
    public class DocumentManipulatorTests
    {
        [TestMethod]
        public void MethodNotTestMethod_DoNotChange()
        {
            var originalCode = "public class Bar { public void MyFancyMethod() {} }";
            var newCode = CategorizeTestMethods(originalCode, "\"Unit test\"");

            Assert.AreEqual(originalCode, newCode.ToString());
        }

        [TestMethod]
        public void TestMethodNotInList_NotCategorized()
        {
            var originalCode = "public class Bar {[TestMethod] public void MyFancyTest() {}}";
            var newCode = CategorizeTestMethods(originalCode, "\"Unit test\"", "MethodToTag");

            Assert.AreEqual(originalCode, newCode.ToString());
        }

        [TestMethod]
        public void TestMethodInList_Categorized()
        {
            var originalCode = "public class Bar {[TestMethod] public void MyFancyTest() {}}";
            var newCode = CategorizeTestMethods(originalCode, "\"Unit test\"", "MyFancyTest");

            Assert.AreNotEqual(originalCode, newCode);
            Assert.IsTrue(newCode.Contains("[TestCategory(\"Unit test\")]"));
        }

        [TestMethod]
        public void TestMethodsInList_CategorizedMultiple()
        {
            var originalCode = "public class Bar {[TestMethod] public void MyFancyTest() {} [TestMethod] public void MySecondTest() {}}";
            var newCode = CategorizeTestMethods(originalCode, "\"Unit test\"", "MyFancyTest", "MySecondTest");

            Assert.AreNotEqual(originalCode, newCode);
            Assert.AreEqual(2, Regex.Matches(newCode, Regex.Escape("[TestCategory(\"Unit test\")]")).Count);
        }

        [TestMethod]
        [TestCategory("Integration test")]
        public void IT_ManipulateSingleFileOnDisk()
        {
            var path = @"..\..\DummyFileForIntegrationTests.cs";
            var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
            var documentManipulator = new DocumentManipulator(syntaxTree);

            documentManipulator.CategorizeTestMethods(new List<string> { "TestMethod1" }, "\"Unit test\"");
            if (documentManipulator.HasRecategorized)
            {
                documentManipulator.SaveChanges(path);
            }
        }

        [TestMethod]
        [TestCategory("Integration test")]
        public void IT_ReadCurrentSolutionCategorizeTestWithRandomCategory()
        {
            string solutionPath = @"..\..\..\TagUnitTestFromTestlist.sln";
            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionPath).Result;
            var projects = solution.Projects.ToList();

            foreach (var project in projects.Where(p => p.Name.Contains("Test")))
            {
                foreach (var document in project.Documents)
                {
                    var documentManipulator = new DocumentManipulator(document, workspace);
                    var guid = System.Guid.NewGuid();
                    documentManipulator.CategorizeTestMethods(new List<string> { "TestMethod1" }, "\"" + guid + "\"");

                    if (documentManipulator.HasRecategorized)
                    {
                        documentManipulator.SaveChanges(document.FilePath);
                    }
                }
            }
        }

        private string CategorizeTestMethods(string originalCode, string testCategoryToAdd, params string[] testToTag)
        {
            Document document;
            using (var adhocWorkspace = new AdhocWorkspace())
            {
                var solution = adhocWorkspace.CurrentSolution;
                var newProject = adhocWorkspace.AddProject("Project", LanguageNames.CSharp);
                document = adhocWorkspace.AddDocument(newProject.Id, "TestFile.cs", SourceText.From(originalCode));

                var documentManipulator = new DocumentManipulator(document, adhocWorkspace);
                documentManipulator.CategorizeTestMethods(testToTag.ToList(), testCategoryToAdd);

                return documentManipulator.SyntaxNode.ToString();
            }
        }
    }
}

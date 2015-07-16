using Microsoft.VisualStudio.TestTools.UnitTesting;
using TagUnitTestFromTestlist.Categorizer;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Linq;

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

        private string CategorizeTestMethods(string originalCode, string testCategoryToAdd, params string[] testToTag)
        {
            Document document;
            using (var adhocWorkspace = new AdhocWorkspace())
            {
                var solution = adhocWorkspace.CurrentSolution;
                var newProject = adhocWorkspace.AddProject("Project", LanguageNames.CSharp);
                document = adhocWorkspace.AddDocument(newProject.Id, "TestFile.cs", SourceText.From(originalCode));

                var documentManipulator = new DocumentManipulator(document, adhocWorkspace);
                var manipulatedSyntaxTree = documentManipulator.CategorizeTestMethods(testToTag.ToList(), testCategoryToAdd);
                
                return manipulatedSyntaxTree.ToString();
            }
        }
    }
}

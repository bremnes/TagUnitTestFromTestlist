using Microsoft.VisualStudio.TestTools.UnitTesting;
using TagUnitTestFromTestlist.Categorizer;

namespace TagUnitTestFromTestlist.Tests
{
    [TestClass]
    public class TestListCategorizerTests
    {
        string solutionPath = @"..\..\..\TagUnitTestFromTestlist.sln";
        string vsmdiPath = @"testlist.vsmdi";

        [TestMethod]
        [TestCategory("Integration test")]
        public void CategorizeAllTests()
        {
            var testListCategorizer = new TestListCategorizer(vsmdiPath, solutionPath);
            testListCategorizer.CategorizeAllTests();
        }

        [TestMethod]
        [TestCategory("Integration test")]
        public void CategorizeIntegrationTests()
        {
            var testListCategorizer = new TestListCategorizer(vsmdiPath, solutionPath);
            testListCategorizer.CategorizeTestList("Integration test");
        }

        [TestMethod]
        [TestCategory("Integration test")]
        public void CategorizeIntegrationTestsSetSpecificCategory()
        {
            var testListCategorizer = new TestListCategorizer(vsmdiPath, solutionPath);
            testListCategorizer.CategorizeTestList("Integration test", "Specific category");
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using TagUnitTestFromTestlist.Models;
using TagUnitTestFromTestlist.Utils;

namespace TagUnitTestFromTestlist.Categorizer
{
    public class TestListCategorizer
    {
        private MSBuildWorkspace _workspace;
        private Solution _solution;
        private List<TestList> _testlists;

        public TestListCategorizer(string pathToVsmdiFile, string pathToSolution)
        {
            _workspace = MSBuildWorkspace.Create();
            _solution = _workspace.OpenSolutionAsync(pathToSolution).Result;
            var vsmdiParser = new VsmdiParser();
            _testlists = vsmdiParser.ReadFile(pathToVsmdiFile);
        }

        public void CategorizeAllTests()
        {
            IEnumerable<TestToCategorize> tests = _testlists.SelectMany(tl => tl.Tests);
            CategorizeTests(tests);
        }

        public void CategorizeTestList(string testListName, string category = "")
        {
            var testlistToprocess = _testlists.SingleOrDefault(tl => tl.Name.Equals(testListName));
            if (testlistToprocess == null)
            {
                throw new ArgumentException(string.Format("Test list {0} does not exist in vsmdi file", testListName));
            }

            if (!string.IsNullOrEmpty(category))
            {
                foreach (var test in testlistToprocess.Tests)
                {
                    test.CategoryName = category;
                }
            }
            
            CategorizeTests(testlistToprocess.Tests);
        }

        public void CategorizeTests(IEnumerable<TestToCategorize> testsToCategorize)
        {
            foreach (var project in _solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var documentManipulator = new DocumentManipulator(document, _workspace);
                    documentManipulator.CategorizeTestMethods(testsToCategorize);

                    if (documentManipulator.HasRecategorized)
                    {
                        documentManipulator.SaveChanges(document.FilePath);
                    }
                }
            }
        }
    }
}

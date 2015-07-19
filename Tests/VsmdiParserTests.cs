using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using TagUnitTestFromTestlist.Utils;

namespace TagUnitTestFromTestlist.Tests
{
    [TestClass]
    public class VsmdiParserTests
    {
        VsmdiParser _vsmdiParser;

        [TestInitialize]
        public void Initialize()
        {
            _vsmdiParser = new VsmdiParser();
        }

        [TestMethod]
        public void ReadFileFromDisk_ReadSuccessfully()
        {
            _vsmdiParser.ReadFile("testlist.vsmdi");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadNonExistingFileFromDisk_ThrowsError()
        {
            _vsmdiParser.ReadFile("non-existing.vsmdi");
        }

        [TestMethod]
        public void ParseTestFileWithThreeLists_ThreeTypedListsReturned()
        {
            var testlists = _vsmdiParser.ReadFile("testlist.vsmdi");
            Assert.AreEqual(3, testlists.Count);
        }

        [TestMethod]
        public void ParseTestFile_HasCorrectNumberOfTestsInTestLists()
        {
            var testlists = _vsmdiParser.ReadFile("testlist.vsmdi");
            var unitTests = testlists.Single(tl => tl.Name.Equals("Unit test")).Tests;
            var integrationTests = testlists.Single(tl => tl.Name.Equals("Integration test")).Tests.Count;
            Assert.AreEqual(2, unitTests.Count);
            Assert.AreEqual(1, integrationTests);
            Assert.AreEqual("Unit test", unitTests.First().CategoryName);
        }
    }
}

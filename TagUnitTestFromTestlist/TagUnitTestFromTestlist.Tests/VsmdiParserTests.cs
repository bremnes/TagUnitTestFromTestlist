using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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
        public void ParseTestFileWithTwoLists_TwoTypedListsReturned()
        {
            var testlists = _vsmdiParser.ReadFile("testlist.vsmdi");
            Assert.AreEqual(2, testlists.Count);
        }
    }
}

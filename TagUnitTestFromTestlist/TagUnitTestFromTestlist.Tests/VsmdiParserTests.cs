using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void CanReadFileFromDisk()
        {
            _vsmdiParser.ReadFile("testlist.vsmdi");
        }
    }
}

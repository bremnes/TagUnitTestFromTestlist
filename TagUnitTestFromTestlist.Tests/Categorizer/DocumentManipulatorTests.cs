using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TagUnitTestFromTestlist.Categorizer;
using TagUnitTestFromTestlist.Tests.Builders;
using Microsoft.CodeAnalysis.Text;

namespace TagUnitTestFromTestlist.Tests.Categorizer
{
    [TestClass]
    public class DocumentManipulatorTests
    {
        [TestMethod]
        public void CodeFileWithoutTestMethods_DoNotChange()
        {
            var originalCode = "public class Bar { public void test() {} }";
            var documentManipulator = new DocumentManipulator(DocumentBuilder.Build("test.cs", originalCode));
            var newDocument = documentManipulator.CategorizeTestMethods();

            SourceText newCode;
            newDocument.TryGetText(out newCode);

            Assert.AreEqual(originalCode, newCode.ToString());
        }
    }
}

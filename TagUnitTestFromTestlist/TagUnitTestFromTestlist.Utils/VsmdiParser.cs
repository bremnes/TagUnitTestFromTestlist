using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TagUnitTestFromTestlist.Models;

namespace TagUnitTestFromTestlist.Utils
{
    public class VsmdiParser
    {
        public List<TestList> ReadFile(string filepath)
        {
            var vsmdiXmlDocument = XDocument.Load(filepath);

            var testlists = vsmdiXmlDocument
                                .Descendants()
                                .Where(d => d.Name.LocalName.Equals("TestList"))
                                .Select(tl => new TestList {
                                    Name = tl.Attribute("name").Value,
                                    Tests = tl.Descendants()
                                                .Where(d => d.Name.LocalName.Equals("TestLink"))
                                                .Select(t => new Test
                                                {
                                                    Name = tl.Attribute("name").Value
                                                })
                                                .ToList()
                                })
                                .ToList();

            return testlists;
        }
    }
}

using System.Xml.Linq;

namespace TagUnitTestFromTestlist.Utils
{
    public class VsmdiParser
    {
        public void ReadFile(string filepath)
        {
            var vsmdiXmlDocument = XDocument.Load(filepath);
        }
    }
}

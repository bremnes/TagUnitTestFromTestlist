using System.Collections.Generic;

namespace TagUnitTestFromTestlist.Models
{
    public class TestList
    {
        public string Name { get; set; }
        public IList<TestToCategorize> Tests { get; set; }
    }
}

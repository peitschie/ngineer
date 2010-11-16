
using System;
using System.Collections.Generic;
namespace NGineer.UnitTests.Types
{
    public class RecursiveClass
    {
        public int IntProperty { get; set; }
        public RecursiveClass RecursiveReference { get; set; }
    }
}

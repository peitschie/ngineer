
using System;
using System.Collections.Generic;
namespace NGineer.UnitTests.Types
{
    public class CountsPropertySets
    {
        private int _somePropertySets;
        public int GetSomePropertySets()
        {
            return _somePropertySets;
        }

        private int _someProperty;
        public int SomeProperty
        {
            get { return _someProperty; }
            set
            {
                _someProperty = value;
                _somePropertySets++;
            }
        }

        private int _recursivePropertySets;
        public int GetRecursivePropertySets()
        {
            return _recursivePropertySets;
        }

        private CountsPropertySets _recursiveProperty;
        public CountsPropertySets RecursiveProperty
        {
            get { return _recursiveProperty; }
            set
            {
                _recursiveProperty = value;
                _recursivePropertySets++;
            }
        }
    }
}


using System;

namespace NGineer.Generators
{
    public class ObjectGenerator : SimpleGenerator<object>
    {
        public ObjectGenerator(int seed) : base(seed)
        {
        }

        protected override object Generate()
        {
            // This default behaviour is probably more intuitive as object is not an easy type to guess!
            return null;
        }
    }
}
using System;

namespace NGineer.BuildGenerators
{
    public class DateTimeGenerator : SimpleGenerator<DateTime>
    {
        public DateTimeGenerator(int seed) : base(seed)
        {
        }

        protected override DateTime Generate()
        {
            return new DateTime(Random.Next());
        }
    }
}
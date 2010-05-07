using System;

namespace NGineer.Generators
{
    public class DateTimeGenerator : SimpleGenerator<DateTime>
    {
        private readonly Random _random;

        public DateTimeGenerator(int seed)
        {
            _random = new Random(seed);
        }

        protected override DateTime Generate()
        {
            return new DateTime(_random.Next());
        }
    }
}
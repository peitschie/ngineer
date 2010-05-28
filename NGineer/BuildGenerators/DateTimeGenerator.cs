using System;

namespace NGineer.BuildGenerators
{
    public class DateTimeGenerator : SimpleGenerator<DateTime>
    {
        protected override DateTime Generate(Random random)
        {
            return new DateTime(random.Next());
        }
    }
}
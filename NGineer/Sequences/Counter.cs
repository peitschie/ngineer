using System;
using NGineer.BuildHelpers;

namespace NGineer.Sequences
{
    public class Counter
    {
        public Func<object, IBuilder, BuildSession, object> FetchValue { get; set; }
        public object LastValue;

        public Counter()
        {}

        public Counter(Func<object, IBuilder, BuildSession, object> fetchValue)
        {
            FetchValue = fetchValue;
        }

        public bool ShouldReset(Counter counter, object obj, IBuilder builder, BuildSession session)
        {
            object newValue = FetchValue(obj, builder, session);
            bool hasChanged = !Equals(LastValue, newValue);
            LastValue = newValue;
            return hasChanged;
        }
    }
}
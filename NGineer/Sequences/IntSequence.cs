using System;
using NGineer.BuildHelpers;

namespace NGineer.Sequences
{
    public class IntSequence<TPropType> : IntSequence<TPropType, object>
    {
        public IntSequence(TPropType properties, Func<TPropType, object, IBuilder, BuildSession, bool> reset)
            : base(properties, reset)
        {
        }
    }

    public class IntSequence<TPropType, TObjType> : ISequence<TObjType>
    {
        private readonly Func<TPropType, object, IBuilder, BuildSession, bool> _reset;
        protected TPropType Properties;
        private int _lastInt = 0;

        public IntSequence(TPropType properties, Func<TPropType, object, IBuilder, BuildSession, bool> reset)
        {
            Properties = properties;
            _reset = reset;
        }

        public object Next(TObjType obj, IBuilder builder, BuildSession session)
        {
            if (_reset != null && _reset(Properties, obj, builder, session))
            {
                _lastInt = 0;
            }
            _lastInt++;
            return _lastInt - 1;
        }
    }

    public class SimpleIntSequence : IntSequence<Counter>
    {
        public SimpleIntSequence(Counter counter) 
            : base(counter, counter.ShouldReset)
        {
        }

        public SimpleIntSequence ResetOnChangeTo(Func<object, IBuilder, BuildSession, object> fetchValue)
        {
            Properties = new Counter(fetchValue);
            return this;
        }
    }
}
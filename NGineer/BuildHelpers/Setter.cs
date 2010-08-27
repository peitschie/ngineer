using System;
using System.Collections.Generic;

namespace NGineer.BuildHelpers
{
    public class Setter<TType> : ISetter
    {
        private readonly Func<TType, IBuilder, BuildSession, TType> _setter;

        public Setter(Func<TType, IBuilder, BuildSession, TType> setter)
        {
            _setter = setter;
        }

        public Setter(Action<TType> setter)
            : this((t, b, s) => { setter(t); return t; })
        {}

        public Setter(Func<TType, TType> setter)
            : this((t, b, s) => setter(t))
        {}

        public Setter(Action<TType, IBuilder, BuildSession> setter)
            : this((t, b, s) => { setter(t, b, s); return t; })
        { }

        public object Set(object obj, IBuilder builder, BuildSession session)
        {
            return _setter((TType)obj, builder, session);
        }

        public bool IsForType(Type type)
        {
            return typeof(TType).IsAssignableFrom(type);
        }
    }
}
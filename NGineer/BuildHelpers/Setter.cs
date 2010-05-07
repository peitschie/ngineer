using System;

namespace NGineer.BuildHelpers
{
    public class Setter<TType> : ISetter
    {
        private readonly Func<TType, IBuilder, TType> _setter;

        public Setter(Func<TType, IBuilder, TType> setter)
        {
            _setter = setter;
        }

        public Setter(Action<TType> setter)
            : this((t, b) => { setter(t); return t; })
        {}

        public Setter(Func<TType, TType> setter)
            : this((t, b) => setter(t))
        {}

        public Setter(Action<TType, IBuilder> setter)
            : this((t, b) => { setter(t, b); return t; })
        { }

        public object Set(object obj, IBuilder builder)
        {
            return _setter((TType)obj, builder);
        }

        public bool IsForType(Type type)
        {
            return typeof(TType).IsAssignableFrom(type);
        }
    }
}
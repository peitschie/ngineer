using System;
using NGineer.Utils;

namespace NGineer.BuildHelpers
{
    public class Setter<T> : ISetter
    {
        private readonly Func<T, T> _setter;

        public Setter(Func<T, T> setter)
        {
            _setter = setter;
        }

        public object Set(object obj)
        {
            return _setter((T)obj);
        }

        public bool IsForType(Type type)
        {
            return typeof (T).IsAssignableFrom(type) || typeof(T).IsGenericAssignableFrom(type);
        }
    }
}
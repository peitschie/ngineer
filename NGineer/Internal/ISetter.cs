using System;

namespace NGineer.Internal
{
    public interface ISetter
    {
        bool IsForType(Type type);
        object Set(object obj, IBuilder builder, BuildSession session);
    }
}
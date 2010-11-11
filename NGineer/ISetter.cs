using System;

namespace NGineer
{
    public interface ISetter
    {
        bool IsForType(Type type);
        object Set(object obj, IBuilder builder, BuildSession session);
    }
}
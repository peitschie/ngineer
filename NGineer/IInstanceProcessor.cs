using System;

namespace NGineer
{
    public interface IInstanceProcessor
    {
        bool IsForType(Type type);
        object Set(object obj, IBuilder builder, BuildSession session);
    }
}
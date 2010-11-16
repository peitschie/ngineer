using System;

namespace NGineer
{
    public interface IInstanceProcessor
    {
        bool IsForType(Type type);
        void Process(object obj, IBuilder builder, BuildSession session);
    }
}
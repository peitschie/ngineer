using System;
using System.Reflection;

namespace NGineer.Internal
{
    public interface IObjectBuildRecord
    {
        Type Type { get; }
        object Object { get; }
        MemberInfo[] UnconstructedMembers { get; }
        bool IsConstructed(MemberInfo member);
        void RegisterConstructed(MemberInfo member);
    }
}
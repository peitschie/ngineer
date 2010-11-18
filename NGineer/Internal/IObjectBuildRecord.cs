using System;
using System.Reflection;

namespace NGineer.Internal
{
    public interface IObjectBuildRecord
    {
        Type Type { get; }
        object Object { get; }
        MemberInfo[] UnconstructedMembers { get; }
        void RegisterConstructed(MemberInfo member);
    }
}
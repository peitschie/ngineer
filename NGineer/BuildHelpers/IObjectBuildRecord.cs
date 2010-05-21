using System;
using System.Reflection;

namespace NGineer.BuildHelpers
{
    public interface IObjectBuildRecord
    {
        Type Type { get; }
        object Object { get; }
        MemberInfo[] UnconstructedMembers { get; }
        bool IsConstructed(MemberInfo property);
        void RegisterConstructed(MemberInfo property);
    }
}
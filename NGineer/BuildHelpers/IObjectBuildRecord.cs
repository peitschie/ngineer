using System;
using System.Reflection;

namespace NGineer.BuildHelpers
{
    public interface IObjectBuildRecord
    {
        Type Type { get; }
        object Object { get; }
        PropertyInfo[] UnconstructedProperties { get; }
        FieldInfo[] UnconstructedFields { get; }
        bool IsConstructed(MemberInfo property);
        void RegisterConstructed(MemberInfo property);
    }
}
using System;
using System.Reflection;
namespace NGineer
{
    public interface IMemberSetter
    {
        bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session);
        void Set(object obj, IBuilder builder, BuildSession session);
    }
}
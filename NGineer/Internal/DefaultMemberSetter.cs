using System;
using NGineer;
using NGineer.Utils;
using System.Reflection;

namespace NGineer.Internal
{
    public class DefaultMemberSetter : IMemberSetter
    {
        public bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session)
        {
            return !session.ShouldIgnoreUnset(session.CurrentObject.Type);
        }

        public void Set(object obj, IBuilder builder, BuildSession session)
        {
            session.CurrentMember.SetValue(obj, builder.Build(session.CurrentMember.ReturnType()));
        }
    }
}


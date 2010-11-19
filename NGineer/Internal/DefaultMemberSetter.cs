using System;
using NGineer.Utils;
using System.Reflection;

namespace NGineer.Internal
{
    public class DefaultMemberSetter : IMemberSetter
    {
        public bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var propertyType = member as PropertyInfo;
                    return propertyType != null && propertyType.GetIndexParameters().Length == 0 && !session.ShouldIgnoreUnset(session.CurrentObject.Type);
                default:
                    return !session.ShouldIgnoreUnset(session.CurrentObject.Type);
            }
        }

        public void Set(object obj, IBuilder builder, BuildSession session)
        {
            session.CurrentMember.SetValue(obj, builder.Build(session.CurrentMember.ReturnType()));
        }
    }
}


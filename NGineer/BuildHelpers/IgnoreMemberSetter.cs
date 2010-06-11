using System;
using System.Reflection;

namespace NGineer.BuildHelpers
{
    public class IgnoreMemberSetter : MemberSetter
    {
        public IgnoreMemberSetter(MemberInfo member) : base(member, (o, b, s) => null)
        {
            if(member == null)
                throw new ArgumentNullException("member");
        }

        public override void Set(object obj, IBuilder builder, BuildSession session)
        {
        }
    }
}
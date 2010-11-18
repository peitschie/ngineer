using System;
using NGineer.Internal;
using System.Reflection;

namespace NGineer.Utils
{
    public static class BuilderUtils
    {
        public static MemberSetter<TObjType, TReturnType> GetMemberSetter<TObjType, TReturnType>(MemberInfo member,
            Func<TObjType, IBuilder, BuildSession, TReturnType> setter, bool allowInherited)
        {
            return new MemberSetter<TObjType, TReturnType>(member, setter, allowInherited);
        }

        public static InstanceProcessor<TObjType> GetSetter<TObjType>(Func<TObjType, IBuilder, BuildSession, TObjType> setter, bool allowInherited)
        {
            return new InstanceProcessor<TObjType>(setter, allowInherited);
        }
    }
}


using System;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.Internal;
using NGineer.Utils;
using System.Collections.Generic;

namespace NGineer.BuildGenerators
{
    public class UniqueCollectionGeneratorMember<TClassType, TType> : IGenerator
    {
        private readonly MemberInfo _member;

        public UniqueCollectionGeneratorMember(Expression<Func<TClassType, TType>> memberInfo)
        {
            _member = memberInfo.GetMemberInfo();
        }

        public UniqueCollectionGeneratorMember(MemberInfo memberInfo)
        {
            if(!typeof(TType).IsAssignableFrom(memberInfo.ReturnType()))
            {
                throw new InvalidCastException(string.Format("Unable to convert member type {0} to {1}", memberInfo.ReturnType(), typeof(TType)));
            }
            _member = memberInfo;
        }
        
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsGenericType && type.IsAssignableFrom(typeof(List<TClassType>));
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            var list = new List<TClassType>();
            if (session.AvailableBuildDepth >= 1)
            {
                foreach (var memberValue in RandomExtensions.Shuffle(EnumUtils.GetValues<TType>(), session.Random))
                {
                    var entry = builder.Build<TClassType>();
                    _member.SetValue(entry, memberValue);
                    list.Add(entry);
                }
            }
            return new ObjectBuildRecord(type, list, false);
        }
    }
}

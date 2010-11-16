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
            _member = MemberExpressions.GetMemberInfo(memberInfo);
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

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            return new List<TClassType>();
        }
        
        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            Populate((IList<TClassType>)obj, builder, session);
        }
        
        protected void Populate(IList<TClassType> list, IBuilder builder, BuildSession session)
        {
            if(session.AvailableBuildDepth >= 1)
            {
                foreach(var memberValue in RandomExtensions.Shuffle<TType>(EnumUtils.GetValues<TType>(), session.Random))
                {
                    var entry = builder.Build<TClassType>();
                    _member.SetValue(entry, memberValue);
                    list.Add(entry);
                }
            }
        }
    }
}

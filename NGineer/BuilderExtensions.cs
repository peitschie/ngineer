
using System;
using NGineer.Internal;
using NGineer.Utils;
using System.Reflection;
using System.Linq.Expressions;
using NGineer.Exceptions;

namespace NGineer
{
    public static class BuilderExtensions
    {
        public static IBuilder AfterConstructionOf<TType>(this IBuilder builder, Expression<Func<TType, object>> expression,
                                                                IGenerator generator)
        {
            if(expression == null)
                throw new ArgumentNullException("expression");
            if(generator == null)
                throw new ArgumentNullException("generator");
            var member = expression.GetMemberInfo();
            // No validation can be done here as the generator only returns a generic object type
            return builder.AfterConstructionOf(new GeneratorMemberSetter(member, typeof(TType), generator, false));
        }

        public static IBuilder IgnoreMember(this IBuilder builder, MemberInfo member, bool allowInherited)
        {
            return builder.AfterConstructionOf(new IgnoreMemberSetter(member, member.ReflectedType, allowInherited));
        }

        public static IBuilder ThrowWhenGeneratingObjectType(this IBuilder builder)
        {
            return builder.WithGenerator<object>((buildr, session) => {
                throw new BuilderException(string.Format("{0}: Object type should not be generated", session.ConstructedChainInfo()));
            });
        }
    }
}


using System;
using System.Linq;
using NGineer.BuildHelpers;
using NGineer.Utils;
using System.Reflection;
using System.Linq.Expressions;
using NGineer.BuildGenerators;

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
            var member = MemberExpressions.GetMemberInfo(expression);
            // No validation can be done here as the generator only returns a generic object type
            return builder.AfterConstructionOf(new GeneratorMemberSetter(member, typeof(TType), generator, false));
        }
    }
}

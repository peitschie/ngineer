
using System;
using System.Collections.Generic;
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

        public static ITypedBuilder<TDictType> AddEntry<TDictType, TKey, TElement>(this ITypedBuilder<TDictType> builder, TKey key, TElement value)
            where TDictType : IDictionary<TKey,TElement>
        {
            return builder.Do(x => x[key] = value);
        }

        public static ITypedBuilder<TType> AddEntry<TType, TDictType, TKey, TElement>(this ITypedBuilder<TType> builder,
            Expression<Func<TType, TDictType>> expression, TKey key, TElement value)
            where TDictType : IDictionary<TKey, TElement>
        {
            var getter = expression.Compile();
            return builder.Do(x =>
            {
                var dict = (IDictionary<TKey, TElement>)getter(x);
                dict[key] = value;
            });
        }

        public static ITypedBuilder<TList> AddEntry<TList, TElement>(this ITypedBuilder<TList> builder,
            TElement value)
            where TList : IList<TElement>
        {
            return builder.Do(x =>
            {
                var list = (IList<TElement>)x;
                list.Add(value);
            });
        }


        public static ITypedBuilder<TType> AddEntry<TType, TList, TElement>(this ITypedBuilder<TType> builder,
            Expression<Func<TType, TList>> expression,
            TElement value)
            where TList : IList<TElement>
        {
            var getter = expression.Compile();
            return builder.Do(x =>
            {
                var list = (IList<TElement>)getter(x);
                list.Add(value);
            });
        }
    }
}

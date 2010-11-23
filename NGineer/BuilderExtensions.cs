
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


        public static ITypedBuilder<TObj> SetConsecutiveInt<TObj>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, int>> property)
        {
            return builder.SetConsecutiveInt(property, 0);
        }


        public static ITypedBuilder<TObj> SetConsecutiveInt<TObj>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, int>> property, int startIndex)
        {
            return builder.Set(property, (obj, buildr, session) => startIndex++);
        }


        public static ITypedBuilder<TObj> SetConsecutiveInt<TObj>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, string>> property)
        {
            return builder.SetConsecutiveInt(property, 0);
        }


        public static ITypedBuilder<TObj> SetConsecutiveInt<TObj>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, string>> property, int startIndex)
        {
            return builder.Set(property, (obj, buildr, session) => startIndex++.ToString());
        }


        public static ITypedBuilder<TObj> SetSiblingConsecutiveInt<TObj>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, int>> property)
        {
            var sequence = Sequences.SiblingConsecutiveInt();
            return builder.Set<int>(property, (o, b, s) => sequence.Next(o, b, s));
        }


        public static TObj Build<TObj>(this ITypedBuilder<TObj> builder)
        {
            return builder.Build<TObj>();
        }


        public static ITypedBuilder<TObj> SetCollectionSize<TObj>(this ITypedBuilder<TObj> builder, int min, int max)
        {
            return builder.SetCollectionSize<TObj>(min, max).For<TObj>();
        }


        public static ITypedBuilder<TObj> Set<TObj, TReturnType>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, TReturnType>> property)
        {
            return builder.Set(property, (o, b, s) => b.Build<TReturnType>()).For<TObj>();
        }


        public static ITypedBuilder<TObj> Set<TObj, TReturnType>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, TReturnType>> property, Func<TReturnType> getter)
        {
            return builder.Set(property, (o, b, s) => getter());
        }


        public static ITypedBuilder<TObj> SetFromCollection<TObj, TReturnType>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, TReturnType>> property, IEnumerable<TReturnType> entries)
        {
            return builder.Set(property, (o, b, s) => s.Random.OneFromList(entries));
        }


        public static ITypedBuilder<TObj> SetString<TObj>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, string>> property, int maxLength)
        {
            return builder.Set(property, (o, b, s) =>
            {
                var str = b.Build<string>();
                return str.Substring(0, Math.Min(str.Length, maxLength));
            });
        }

        /// <summary>
        /// WithGenerator extension that uses a randomly selected sub-type from the provided types list every time the templated type is to be
        /// constructed.  Useful when a given class is an abstract parent class
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="builder"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IBuilder WithGenerator<TType>(this IBuilder builder, params Type[] types)
        {
            foreach (var type in types)
            {
                if (!typeof(TType).IsAssignableFrom(type))
                    throw new InvalidCastException(string.Format("Cannot cast {0} to {1}", type, typeof(TType)));
            }
            return builder.WithGenerator<TType>((buildr, session) => (TType)buildr.Build(session.Random.OneFromList((IEnumerable<Type>)types)));
        }


        public static ITypedBuilder<TObj> SetAsType<TObj, TReturnType>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, TReturnType>> property, params Type[] types)
        {
            return builder.Set(property, (o, b, s) => (TReturnType)b.Build(s.Random.OneFromList((IEnumerable<Type>)types)));
        }
    }
}

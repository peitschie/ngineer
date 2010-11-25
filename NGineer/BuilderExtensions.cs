
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


        /// <summary>
        /// Throw an exception any time a plain object type is constructed
        /// </summary>
        /// <param name="builder">
        /// A <see cref="IBuilder"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        public static IBuilder ThrowWhenGeneratingObjectType(this IBuilder builder)
        {
            return builder.WithGenerator<object>((buildr, session) => {
                throw new BuilderException("Object type should not be generated");
            });
        }

        /// <summary>
        /// Add a given key / value pair into the selected dictionary type
        /// Other key and value pairs may already exist
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TDictType>"/>
        /// </param>
        /// <param name="key">
        /// A <see cref="TKey"/>
        /// </param>
        /// <param name="value">
        /// A <see cref="TElement"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TDictType>"/>
        /// </returns>
        public static ITypedBuilder<TDictType> AddEntry<TDictType, TKey, TElement>(this ITypedBuilder<TDictType> builder, TKey key, TElement value)
            where TDictType : IDictionary<TKey,TElement>
        {
            return builder.Do(x => x[key] = value);
        }

        /// <summary>
        /// Add a given key / value pair into the selected member's dictionary
        /// Other key and value pairs may already exist
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </param>
        /// <param name="expression">
        /// A <see cref="Expression<Func<TType, TDictType>>"/>
        /// </param>
        /// <param name="key">
        /// A <see cref="TKey"/>
        /// </param>
        /// <param name="value">
        /// A <see cref="TElement"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
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

        /// <summary>
        /// Add a given value into the selected list type
        /// Other entries may already be present in the list
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TList>"/>
        /// </param>
        /// <param name="value">
        /// A <see cref="TElement"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TList>"/>
        /// </returns>
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

        /// <summary>
        /// Add a given value into the selected member's list
        /// Other entries may already be present in the list
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </param>
        /// <param name="expression">
        /// A <see cref="Expression<Func<TType, TList>>"/>
        /// </param>
        /// <param name="value">
        /// A <see cref="TElement"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
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

        /// <summary>
        /// Set the member with a sequentially increasing number starting from 0
        /// This number is not reset when a new build session is started
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </param>
        /// <param name="property">
        /// A <see cref="Expression<Func<TObj, System.Int32>>"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </returns>
        public static ITypedBuilder<TObj> SetConsecutiveInt<TObj>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, int>> property)
        {
            return builder.SetConsecutiveInt(property, 0);
        }

        /// <summary>
        /// Set the member with a sequentially increasing number starting from the specified starting point
        /// This number is not reset when a new build session is started
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </param>
        /// <param name="property">
        /// A <see cref="Expression<Func<TObj, System.Int32>>"/>
        /// </param>
        /// <param name="startIndex">
        /// Starting number to commence number from.  The first created object receives this value
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </returns>
        public static ITypedBuilder<TObj> SetConsecutiveInt<TObj>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, int>> property, int startIndex)
        {
            return builder.Set(property, (obj, buildr, session) => startIndex++);
        }

        /// <summary>
        /// Numbers sibling items with the same immediate parent with a sequentially increasing number starting from 0
        /// This number is reset to 0 every new parent object, and remembers the last number used for a given parent hierarchy
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </param>
        /// <param name="property">
        /// A <see cref="Expression<Func<TObj, System.Int32>>"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </returns>
        public static ITypedBuilder<TObj> SetSiblingConsecutiveInt<TObj>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, int>> property)
        {
            var sequence = Sequences.SiblingConsecutiveInt();
            return builder.Set<int>(property, (o, b, s) => sequence.Next(o, b, s));
        }


        public static ITypedBuilder<TObj> SetCollectionSize<TObj>(this ITypedBuilder<TObj> builder, int min, int max)
        {
            return builder.SetCollectionSize<TObj>(min, max).For<TObj>();
        }

        /// <summary>
        /// Populate a member using the sessioned builder
        /// This is intended to allow easy restoring after ignoring all members on a given type
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </param>
        /// <param name="property">
        /// A <see cref="Expression<Func<TObj, TReturnType>>"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </returns>
        public static ITypedBuilder<TObj> Set<TObj, TReturnType>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, TReturnType>> property)
        {
            return builder.Set(property, (o, b, s) => b.Build<TReturnType>());
        }

        /// <summary>
        /// Simple factory overload that populates a member with the given result of the parameterless getter
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </param>
        /// <param name="property">
        /// A <see cref="Expression<Func<TObj, TReturnType>>"/>
        /// </param>
        /// <param name="getter">
        /// A <see cref="Func<TReturnType>"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </returns>
        public static ITypedBuilder<TObj> Set<TObj, TReturnType>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, TReturnType>> property, Func<TReturnType> getter)
        {
            return builder.Set(property, (o, b, s) => getter());
        }

        /// <summary>
        /// Set the value of a member from a specified list of options (repeats allowed)
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </param>
        /// <param name="property">
        /// A <see cref="Expression<Func<TObj, TReturnType>>"/>
        /// </param>
        /// <param name="entries">
        /// A <see cref="IEnumerable<TReturnType>"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </returns>
        public static ITypedBuilder<TObj> SetFromCollection<TObj, TReturnType>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, TReturnType>> property, IEnumerable<TReturnType> entries)
        {
            return builder.Set(property, (o, b, s) => s.Random.OneFromList(entries));
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


        /// <summary>
        /// Set the member with a constructed instance of one of the specified types
        /// A random type is selected each time a member is constructed
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </param>
        /// <param name="property">
        /// A <see cref="Expression<Func<TObj, TReturnType>>"/>
        /// </param>
        /// <param name="types">
        /// A <see cref="Type[]"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TObj>"/>
        /// </returns>
        public static ITypedBuilder<TObj> SetAsType<TObj, TReturnType>(this ITypedBuilder<TObj> builder, Expression<Func<TObj, TReturnType>> property, params Type[] types)
        {
            return builder.Set(property, (o, b, s) => (TReturnType)b.Build(s.Random.OneFromList((IEnumerable<Type>)types)));
        }
    }
}


using System;
using System.Linq.Expressions;

namespace NGineer
{
    public interface ITypedBuilder<TType> : IBuilder
    {
        /// <summary>
        /// Build an instance of the TypedBuilder type
        /// </summary>
        /// <returns>
        /// A <see cref="TType"/>
        /// </returns>
        TType Build();

        /// <summary>
        /// Register a member setter for a specific property or field
        /// </summary>
        /// <param name="setter">
        /// A <see cref="IMemberSetter"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        ITypedBuilder<TType> Set<TReturnType>(Expression<Func<TType, TReturnType>> expression, Func<TType, IBuilder, BuildSession, TReturnType> value);

        /// <summary>
        /// Register a member setter for a specific property or field
        /// </summary>
        /// <param name="expression">
        /// A <see cref="Expression<Func<TType, TReturnType>>"/>
        /// </param>
        /// <param name="value">
        /// A <see cref="TReturnType"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
        ITypedBuilder<TType> Set<TReturnType>(Expression<Func<TType, TReturnType>> expression, TReturnType value);

        /// <summary>
        /// Register a member setter for a specific property or field
        /// </summary>
        /// <param name="expression">
        /// A <see cref="Expression<Func<TType, TReturnType>>"/>
        /// </param>
        /// <param name="value">
        /// A <see cref="Func<TReturnType>"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
		ITypedBuilder<TType> Set<TReturnType>(Expression<Func<TType, TReturnType>> expression, Func<TReturnType> value);

        /// <summary>
        /// Register a member setter for a specific property or field
        /// </summary>
        /// <param name="expression">
        /// A <see cref="Expression<Func<TType, System.Object>>"/>
        /// </param>
        /// <param name="generator">
        /// A <see cref="IGenerator"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
        ITypedBuilder<TType> Set(Expression<Func<TType, object>> expression, IGenerator generator);

        /// <summary>
        /// A delayed setter that is executed after the object the has been constructed
        /// This is useful when the the object depends on other members being constructed first
        /// </summary>
        /// <param name="expression">
        /// A <see cref="Expression<Func<TType, TReturnType>>"/>
        /// </param>
        /// <param name="value">
        /// A <see cref="Func<BuildSession, TReturnType>"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
        ITypedBuilder<TType> SetAfterBuild<TReturnType>(Expression<Func<TType, TReturnType>> expression, Func<BuildSession, TReturnType> value);

        /// <summary>
        /// A delayed setter that is executed after the object the has been constructed
        /// This is useful when the the object depends on other members being constructed first
        /// </summary>
        /// <param name="expression">
        /// A <see cref="Expression<Func<TType, TReturnType>>"/>
        /// </param>
        /// <param name="value">
        /// A <see cref="Func<TType, BuildSession, TReturnType>"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
        ITypedBuilder<TType> SetAfterBuild<TReturnType>(Expression<Func<TType, TReturnType>> expression, Func<TType, BuildSession, TReturnType> value);

        ITypedBuilder<TType> Ignore(Expression<Func<TType, object>> expression);
        ITypedBuilder<TType> Ignore<TReturnType>(Expression<Func<TType, TReturnType>> expression);
        ITypedBuilder<TType> IgnoreUnset();
        ITypedBuilder<TType> IgnoreAll();

        /// <summary>
        /// Registers an action that is called after an object of this type is constructed
        /// </summary>
        /// <param name="setter">
        /// A <see cref="ISetter"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        ITypedBuilder<TType> Do(Action<TType> setter);

        /// <summary>
        /// Registers an action that is called after an object of this type is constructed
        /// </summary>
        /// <param name="setter">
        /// A <see cref="Action<TType, IBuilder, BuildSession>"/>
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
        ITypedBuilder<TType> Do(Action<TType, IBuilder, BuildSession> setter);

        /// <summary>
        /// Specifies the maximum number of unique instances of this type to create before re-using existing instances
        /// </summary>
        /// <param name="minimum">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="maximum">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        ITypedBuilder<TType> SetCollectionSize(int min, int max);
    }
}


using System;
using System.Linq.Expressions;
using NGineer.BuildHelpers;
using NGineer.BuildGenerators;

namespace NGineer
{
    public interface ITypedBuilder<TType> : IBuilder
    {
        /// <summary>
        /// Registers a post-constructor member setter to fill out a specific property or field.
        /// The builder's parent setters are called prior to calling the current setters.
        ///
        /// Once a property or field has been set, it will not get populated in the next stage.  Post-population
        /// steps will still be run on this object however.
        /// </summary>
        /// <param name="setter">
        /// A <see cref="IMemberSetter"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        ITypedBuilder<TType> Set<TReturnType>(Expression<Func<TType, TReturnType>> expression, Func<TType, IBuilder, BuildSession, TReturnType> value);
        ITypedBuilder<TType> Set<TReturnType>(Expression<Func<TType, TReturnType>> expression, TReturnType value);
        ITypedBuilder<TType> Set(Expression<Func<TType, object>> expression, IGenerator generator);

        ITypedBuilder<TType> Ignore(Expression<Func<TType, object>> expression);

        /// <summary>
        /// Registers a post-population member that is called after the object has been constructed and populated
        /// by previous construction setters and object generators.
        /// </summary>
        /// <param name="setter">
        /// A <see cref="ISetter"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        ITypedBuilder<TType> Do(Action<TType> setter);
        ITypedBuilder<TType> Do(Action<TType, IBuilder, BuildSession> setter);

        /// <summary>
        /// Specifies the maximum number of unique instances of a certain
        /// type to create before simply re-using existing types. 
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
        ITypedBuilder<TType> SetNumberOfInstances(int minimum, int maximum);
    }
}

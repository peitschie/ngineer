using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.BuildGenerators;
using NGineer.Internal;

namespace NGineer
{
    public interface IBuilder
    {
        #region Build and overloads

        /// <summary>
        /// Constructs a new instance of the passed in type.  As a by-product,
        /// this will seal this builder class and any parent classes, disallowing any
        /// direct modification from this point onwards.
        /// </summary>
        /// <param name="type">
        /// A <see cref="Type"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Object"/>
        /// </returns>
        object Build(Type type);
		TType Build<TType>();
		TType Build<TType>(BuildSession session);

        /// <summary>
        /// Constructs a new instance of the passed in type re-using the specified
        /// session.  This is generally only used when chaining builders within a session.
        /// As a by-product, this will seal this builder class and any parent classes, disallowing any
        /// direct modification from this point onwards.
        /// </summary>
        /// <param name="type">
        /// A <see cref="Type"/>
        /// </param>
        /// <param name="session">
        /// A <see cref="BuildSession"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Object"/>
        /// </returns>
        object Build(Type type, BuildSession session);
        #endregion

        /// <summary>
        /// Set or clear the maximum build depth limit.  This is the number
        /// of levels to populate down the object tree before stopping and returning
        /// null. A level of 0 builds just the selected object type (A).  A level of 1 will
        /// populate the properties directly below the root object (A.Property).  A level of 2 will
        /// populate the properties' properties (A.Property.Property)
        /// </summary>
        /// <param name="depth">
        /// A <see cref="System.Nullable<System.Int32>"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        IBuilder SetMaximumDepth(int? depth);

        IBuilder SetMaximumObjects(int? maximum);

        /// <summary>
        /// By default, if the specified maximum build depth is reached, the builder will simply return null for anything
        /// deeper.  This method can be used to force the builder to throw an exception if the maximum build depth is reached.
        /// This can be useful for ensuring a complete object hierarchy is being constructed
        /// </summary>
        /// <returns></returns>
        IBuilder ThrowsWhenMaximumDepthReached();

        /// <summary>
        /// Inject a custom generator to use for creating and populating new objects.
        /// These are injected in order, with the most recently injected generator taking
        /// the highest precedence first.
        /// </summary>
        /// <param name="generator">
        /// A <see cref="IGenerator"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        IBuilder WithGenerator(IGenerator generator);
		IBuilder WithGenerator(Type type, Func<IBuilder, BuildSession, object> generator);
		IBuilder WithGenerator<TType>(Func<IBuilder, BuildSession, TType> generator);
		IBuilder WithGenerator<TType>(Func<TType> generator);

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
		IBuilder AfterConstructionOf(IMemberSetter setter);

        /// <summary>
        /// Ignores any members for this object type that do not currently have an existing setter
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        IBuilder IgnoreUnset(Type type);

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
		IBuilder AfterPopulationOf(ISetter setter);

        /// <summary>
        /// Sets the range of items to put in an array, list or other type of collection by default.
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
        IBuilder SetDefaultCollectionSize(int minimum, int maximum);

        /// <summary>
        /// Specify the number of items to put a list or collection of a specific type.
        /// </summary>
        /// <param name="type">
        /// A <see cref="Type"/>
        /// </param>
        /// <param name="minimum">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="maximum">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        IBuilder SetCollectionSize(Type type, int minimum, int maximum);
		IBuilder SetCollectionSize<TType>(int minimum, int maximum);

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
        IBuilder SetNumberOfInstances(Type type, int minimum, int maximum);
		IBuilder SetNumberOfInstances<TType>(int minimum, int maximum);

        ITypedBuilder<TType> For<TType>();
		ITypedBuilder<TType> For<TType>(bool allowInherited);

        /// <summary>
        /// Indicates an action that will be run after all construction and population
        /// has been completed.  It is HIGHLY recommended no further building be done at
        /// this point
        /// </summary>
        /// <param name="hook">
        /// A <see cref="Action<BuildSession>"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        IBuilder PostBuild(Action<BuildSession> hook);

        IBuilder CreateNew();

        /// <summary>
        /// Marks this builder as sealed.  This prevents accidently overriding values, depths or generators.
        /// It is highly recommended that all builders be sealed after initial creation to prevent later tests
        /// from modifying the container behaviour
        /// </summary>
        /// <returns></returns>
        IBuilder Sealed();
		
		int BuildDepth { get; }
    }
}

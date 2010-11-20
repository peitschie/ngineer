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
        /// Constructs a new instance of the requested in type.  As a by-product,
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

        /// <summary>
        /// Constructs a new instance of the requested type.  As a by-product,
        /// this will seal this builder class and any parent classes, disallowing any
        /// direct modification from this point onwards.
        /// </summary>
        /// <param name="type">
        /// A <see cref="Type"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Object"/>
        /// </returns>
		TType Build<TType>();
        #endregion

        /// <summary>
        /// Set the maximum number of levels to populate
        /// This is the number of levels to populate down the object tree before stopping
        /// and returning null.
        /// A level of 0 builds just the selected object type (A).
        /// A level of 1 will populate the properties directly below the root object (A.Property).
        /// A level of 2 will populate the properties' properties (A.Property.Property)
        /// </summary>
        /// <param name="depth">
        /// A <see cref="System.Nullable<System.Int32>"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        IBuilder SetMaximumDepth(int? depth);

        /// <summary>
        /// Set the maximum number of instances to create before throwing an exception
        /// This is used to help tune builder performance for tests that may accidentally
        /// build large amounts of data (e.g., a parent that references a child that has a back
        /// reference to the parent)
        /// </summary>
        /// <param name="maximum">
        /// A <see cref="System.Nullable<System.Int32>"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        IBuilder SetMaximumObjects(int? maximum);

        /// <summary>
        /// Causes the builder to throw an exception if the maximum build depth is ever reached
        /// By default, if the specified maximum build depth is reached, the builder will simply return null for anything
        /// deeper.  This method can be used to force the builder to throw an exception if the maximum build depth is reached.
        /// This can be useful for ensuring a complete object hierarchy is being constructed
        /// </summary>
        /// <returns></returns>
        IBuilder ThrowsWhenMaximumDepthReached();

        /// <summary>
        /// Adds a custom generator to use for creating new objects
        /// The most recently injected generator is used first
        /// </summary>
        /// <param name="generator">
        /// A <see cref="IGenerator"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        IBuilder WithGenerator(IGenerator generator);

        /// <summary>
        /// Adds a custom generator to use for creating new objects
        /// The most recently injected generator is used first
        /// </summary>
        /// <param name="generator">
        /// A <see cref="IGenerator"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
		IBuilder WithGenerator(Type type, Func<IBuilder, BuildSession, object> generator);

        /// <summary>
        /// Adds a custom generator to use for creating new objects
        /// The most recently injected generator is used first
        /// </summary>
        /// <param name="generator">
        /// A <see cref="IGenerator"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
		IBuilder WithGenerator<TType>(Func<IBuilder, BuildSession, TType> generator);

        /// <summary>
        /// Adds a custom generator to use for creating new objects
        /// The most recently injected generator is used first
        /// </summary>
        /// <param name="generator">
        /// A <see cref="IGenerator"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
		IBuilder WithGenerator<TType>(Func<TType> generator);

        /// <summary>
        /// Registers a member setter to fill out a specific property or field.
        /// Only a single setter is called for each member on an object.
        /// The builder's setters are checked for a match prior to the parent's setters being checked
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
        /// <returns></returns>
        IBuilder IgnoreUnset(Type type);

        /// <summary>
        /// Registers a processor that is called after an object has been constructed
        /// </summary>
        /// <param name="setter">
        /// A <see cref="ISetter"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
		IBuilder AfterPopulationOf(IInstanceProcessor setter);

        /// <summary>
        /// Sets the range of items to put in an array, list or other type of collection
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
		IBuilder SetCollectionSize<TType>(int minimum, int maximum);

        /// <summary>
        /// Specifies the maximum number of unique instances of a certain type to create before re-using existing types.
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

        /// <summary>
        /// Specifies the maximum number of unique instances of a certain type to create before re-using existing types.
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
		IBuilder SetNumberOfInstances<TType>(int minimum, int maximum);

        /// <summary>
        /// Configure a type-specific builder. This allows easy access to various type-specific setters and processors
        /// These settings are not applied to inherited types
        /// </summary>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
        ITypedBuilder<TType> For<TType>();

        /// <summary>
        /// Configure a type-specific builder. This allows easy access to various type-specific setters and processors
        /// These settings are applied to inherited types if the allowInherited parameter is true
        /// </summary>
        /// <param name="allowInherited">
        /// A <see cref="System.Boolean"/>.  If set to true, configurations on this builder are applied to any
        /// constructed type that is assignable to the specified type
        /// </param>
        /// <returns>
        /// A <see cref="ITypedBuilder<TType>"/>
        /// </returns>
		ITypedBuilder<TType> For<TType>(bool allowInherited);

        /// <summary>
        /// Register an action that will be run after all construction and population has been completed
        /// this point
        /// </summary>
        /// <param name="hook">
        /// A <see cref="Action<BuildSession>"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        IBuilder PostBuild(Action<BuildSession> hook);

        /// <summary>
        /// Create a child builder.  All parent settings and actions are inherited
        /// Further reconfiguration of the parent will affect the child builder up until
        /// the child builder is sealed
        /// </summary>
        /// <returns>
        /// A <see cref="IBuilder"/>
        /// </returns>
        IBuilder CreateNew();

        /// <summary>
        /// Marks this builder as sealed. This prevents accidently overriding values, depths or generators.
        /// Sealing a child builder will also seal all parent builders
        /// </summary>
        /// <returns></returns>
        IBuilder Sealed();

        int BuildDepth { get; }
    }
}

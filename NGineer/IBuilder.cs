using System;
using System.Linq;
using System.Linq.Expressions;
using NGineer.BuildHelpers;
using NGineer.Generators;

namespace NGineer
{
    public interface IBuilder
    {
        object Build(Type type);
        object Build(Type type, BuildSession session);
        TType Build<TType>();
        TType Build<TType>(BuildSession session);

        IBuilder SetMaximumDepth(int? depth);
        IBuilder SetCollectionSize(int minimum, int maximum);

        IBuilder WithGenerator(IGenerator generator);

        IBuilder AfterConstructionOf<TType>(Expression<Func<TType, object>> expression, Func<object, IBuilder, BuildSession, object> value);
        IBuilder AfterConstructionOf<TType, TCallType>(Expression<Func<TType, object>> expression, Func<TCallType, IBuilder, BuildSession, object> value);
        IBuilder AfterConstructionOf<TType>(Expression<Func<TType, object>> expression, object value);

        IBuilder AfterPopulationOf<TType>(Action<TType> setter);
        IBuilder AfterPopulationOf<TType>(Func<TType, TType> setter);
        IBuilder AfterPopulationOf<TType>(Action<TType, IBuilder, BuildSession> setter);
        IBuilder AfterPopulationOf<TType>(Func<TType, IBuilder, BuildSession, TType> setter);
        IBuilder AfterPopulationOf(ISetter setter);

        IBuilder SetCollectionSize<TType>(int minimum, int maximum);
		IBuilder SetNumberOfInstances<TType>(int minimum, int maximum);

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

    public interface IBuilder<TBuildType> : IBuilder
	{
        TBuildType Build();
        TBuildType Build(BuildSession session);

        new IBuilder<TBuildType> SetMaximumDepth(int? depth);
        new IBuilder<TBuildType> SetCollectionSize(int minimum, int maximum);

        new IBuilder<TBuildType> WithGenerator(IGenerator generator);
        
        new IBuilder<TBuildType> AfterConstructionOf<TType>(Expression<Func<TType, object>> expression, Func<object, IBuilder, BuildSession, object> value);
        new IBuilder<TBuildType> AfterConstructionOf<TType, TCallType>(Expression<Func<TType, object>> expression, Func<TCallType, IBuilder, BuildSession, object> value);
        new IBuilder<TBuildType> AfterConstructionOf<TType>(Expression<Func<TType, object>> expression, object value);

        new IBuilder<TBuildType> AfterPopulationOf<TType>(Action<TType> setter);
        new IBuilder<TBuildType> AfterPopulationOf<TType>(Func<TType, TType> setter);
        new IBuilder<TBuildType> AfterPopulationOf<TType>(Action<TType, IBuilder, BuildSession> setter);
        new IBuilder<TBuildType> AfterPopulationOf<TType>(Func<TType, IBuilder, BuildSession, TType> setter);
        new IBuilder<TBuildType> AfterPopulationOf(ISetter setter);

        new IBuilder<TBuildType> SetCollectionSize<TType>(int minimum, int maximum);
        new IBuilder<TBuildType> SetNumberOfInstances<TType>(int minimum, int maximum);

        new IBuilder<TBuildType> CreateNew();

        new IBuilder<TBuildType> Sealed();
	}
}

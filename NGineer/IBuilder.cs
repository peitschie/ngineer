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

		IBuilder AfterConstructionOf(IMemberSetter setter);
        IBuilder AfterConstructionOf<TType>(Expression<Func<TType, object>> expression, Func<object, IBuilder, BuildSession, object> value);
        IBuilder AfterConstructionOf<TType, TCallType>(Expression<Func<TType, object>> expression, Func<TCallType, IBuilder, BuildSession, object> value);
        IBuilder AfterConstructionOf<TType>(Expression<Func<TType, object>> expression, object value);

		IBuilder AfterPopulationOf(ISetter setter);
        IBuilder AfterPopulationOf<TType>(Action<TType> setter);
        IBuilder AfterPopulationOf<TType>(Func<TType, TType> setter);
        IBuilder AfterPopulationOf<TType>(Action<TType, IBuilder, BuildSession> setter);
        IBuilder AfterPopulationOf<TType>(Func<TType, IBuilder, BuildSession, TType> setter);

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
}

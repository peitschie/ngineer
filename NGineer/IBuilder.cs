using System;
using System.Linq;
using NGineer.Generators;

namespace NGineer
{
    public interface IBuilder
    {
        object Build(Type type);
        IBuilder WithGenerator(IGenerator generator);
        IBuilder SetMaximumDepth(int depth);
        IBuilder CreateNew();
        TType Build<TType>();

        IBuilder SetValuesFor<TType>(Action<TType> setter);
        IBuilder SetValuesFor<TType>(Func<TType, TType> setter);
        IBuilder SetValuesFor<TType>(Action<TType, IBuilder> setter);
        IBuilder SetValuesFor<TType>(Func<TType, IBuilder, TType> setter);

        /// <summary>
        /// Marks this builder as sealed.  This prevents accidently overriding values, depths or generators.
        /// It is highly recommended that all builders be sealed after initial creation to prevent later tests
        /// from modifying the container behaviour
        /// </summary>
        /// <returns></returns>
        IBuilder Seal();
    }

    public interface IBuilder<out TBuildType> : IBuilder
	{
        new IBuilder<TBuildType> WithGenerator(IGenerator generator);
        new IBuilder<TBuildType> SetMaximumDepth(int depth);
        new IBuilder<TBuildType> CreateNew();
        TBuildType Build();

        new IBuilder<TBuildType> SetValuesFor<TType>(Action<TType> setter);
        new IBuilder<TBuildType> SetValuesFor<TType>(Func<TType, TType> setter);
        new IBuilder<TBuildType> SetValuesFor<TType>(Action<TType, IBuilder> setter);
        new IBuilder<TBuildType> SetValuesFor<TType>(Func<TType, IBuilder, TType> setter);

        new IBuilder<TBuildType> Seal();
	}
}

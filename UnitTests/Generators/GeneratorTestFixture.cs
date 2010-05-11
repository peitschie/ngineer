using System;
using System.Collections.Generic;
using System.Reflection;
using NGineer.BuildHelpers;
using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
	public abstract class GeneratorTestFixture<TGenerator>
        where TGenerator : IGenerator
	{
	    protected TGenerator Generator;
        
        [SetUp]
        public void GeneratorTestFixture_SetUp()
        {
            Generator = Construct();
        }

        protected bool GeneratesType(Type type)
        {
            return Generator.GeneratesType(type, null, null);
        }

        protected TType CreateAndGenerate<TType>(IBuilder builder, BuildSession session)
        {
            return CreateAndGenerate<TType>(Generator, builder, session);
        }

        protected TType CreateAndGenerate<TType>(IGenerator generator, IBuilder builder, BuildSession session)
        {
            var obj = (TType)generator.Create(typeof(TType), builder, session);
            return (TType)generator.Populate(typeof(TType), obj, builder, session);
        }

        protected virtual TGenerator Construct()
        {
            var seededConstructor = typeof (TGenerator).GetConstructor(new[] {typeof (int)});
            if (seededConstructor == null)
            {
                var defaultConstructor = typeof (TGenerator).GetConstructor(new Type[0]);
                if (defaultConstructor == null)
                    throw new Exception("Unable to find appropriate constructor for generator");
                return (TGenerator) defaultConstructor.Invoke(new object[0]);
            }
            return (TGenerator) seededConstructor.Invoke(new object[] {100});
        }

	    public abstract void GeneratesTypes_AcceptsTypes();
		public abstract void GeneratesTypes_RejectsTypes();
	}
}

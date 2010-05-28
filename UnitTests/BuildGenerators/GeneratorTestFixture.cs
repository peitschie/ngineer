using System;
using Moq;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.Utils;
using NUnit.Framework;
using Range = NGineer.BuildHelpers.Range;

namespace NGineer.UnitTests.BuildGenerators
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

        [Test]
        public void GeneratesTypes_AcceptsTypes()
        {
            foreach (var supportedType in SupportedTypes())
            {
                Assert.IsTrue(GeneratesType(supportedType), "Expected type {0} was not supported".With(supportedType));    
            }
        }

        [Test]
        public void GeneratesTypes_RejectsTypes()
        {
            foreach (var unsupportedType in UnsupportedTypes())
            {
                Assert.IsFalse(GeneratesType(unsupportedType), "Unexpected type {0} was supported".With(unsupportedType));
            }
        }

        [Test]
        public virtual void Create_AllSupportedTypes()
        {
            var builder = new Mock<IBuilder>();
            builder.Setup(b => b.Build(It.IsAny<Type>())).Throws(new BuilderCalledException());
            foreach (var supportedType in SupportedTypes())
            {
                try
                {
                    Generator.Create(supportedType, builder.Object, BuildSession());
                }
                catch(BuilderCalledException)
                {}
                catch(Exception e)
                {
                    Assert.Fail("Unable to construct {0}: {1}", supportedType, e);
                }
            }
        }

        private class BuilderCalledException : Exception
        {
        }

        protected virtual bool GeneratesType(Type type)
        {
            return Generator.GeneratesType(type, null, BuildSession());
        }

        protected TType CreateAndGenerate<TType>(IBuilder builder, BuildSession session)
        {
            return CreateAndGenerate<TType>(Generator, builder, session);
        }

        protected TType CreateAndGenerate<TType>(IGenerator generator, IBuilder builder, BuildSession session)
        {
			Assert.IsTrue(generator.GeneratesType(typeof(TType), builder, session), "Does not generate type {0}".With(typeof(TType)));
            var obj = (TType)generator.Create(typeof(TType), builder, session);
            generator.Populate(typeof(TType), obj, builder, session);
            return obj;
        }

		protected virtual BuildSession BuildSession()
		{
			var defaultRange = new Range(1, 10);
            var collectionTypes = new TypeRegistry<Range>();
			var maxInstances = new TypeRegistry<int?>();
			return new BuildSession(null, collectionTypes, maxInstances, defaultRange, new Random(10));
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

	    protected abstract Type[] SupportedTypes();
		protected abstract Type[] UnsupportedTypes();
	}
}

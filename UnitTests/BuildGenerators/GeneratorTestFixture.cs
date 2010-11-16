using System;
using Moq;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NGineer.Utils;
using NUnit.Framework;
using Range = NGineer.Internal.Range;
using NGineer.UnitTests.Types;

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
                Assert.IsTrue(GeneratesType(supportedType), string.Format("Expected type {0} was not supported", supportedType));
            }
        }

        [Test]
        public void GeneratesTypes_RejectsTypes()
        {
            foreach (var unsupportedType in UnsupportedTypes())
            {
                Assert.IsFalse(GeneratesType(unsupportedType), string.Format("Unexpected type {0} was supported", unsupportedType));
            }
        }

        [Test]
        public virtual void Create_AllSupportedTypes()
        {
            var builder = GetBuilder();
            foreach (var supportedType in SupportedTypes())
            {
                try
                {
                    Generator.Create(supportedType, builder, BuildSession());
                }
                catch (BuilderCalledException)
                {
                }
                catch (Exception e)
                {
                    Assert.Fail("Unable to construct {0}: {1}", supportedType, e);
                }
            }
        }

        protected virtual IBuilder GetBuilder()
        {
            var builder = new Mock<IBuilder>();
            builder.Setup(b => b.Build(It.IsAny<Type>())).Throws(new BuilderCalledException());
            return builder.Object;
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
			Assert.IsTrue(generator.GeneratesType(typeof(TType), builder, session), string.Format("Does not generate type {0}", typeof(TType)));
            var obj = (TType)generator.Create(typeof(TType), builder, session);
            generator.Populate(typeof(TType), obj, builder, session);
            return obj;
        }

		protected virtual BuildSession BuildSession()
		{
            var builder = new TestBuilder {
                DefaultCollectionSize = new Range(1, 10),
                CollectionSizes = new TypeRegistry<Range>(),
                MaxInstances = new TypeRegistry<int?>(),
                BuildDepth = 10
            };
			return new BuildSession(builder, null, new Random(10));
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

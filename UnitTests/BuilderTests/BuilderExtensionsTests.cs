using System;
using NUnit.Framework;
using NGineer.Exceptions;
using NGineer.UnitTests.Types;
namespace NGineer.UnitTests.BuilderTests
{
    [TestFixture]
    public class BuilderExtensionsTests
    {
        private IBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new Builder();
        }

        [Test]
        public void ThrowWhenGeneratingObjectType_ThrowsIfObjectIsConstructed()
        {
            _builder.ThrowWhenGeneratingObjectType();
            var wrappedException = Assert.Throws<WrappedBuilderException>(() => _builder.Build<ClassWithObject>());
            Assert.IsInstanceOf<BuilderException>(wrappedException.InnerException);
            var exception = (BuilderException)wrappedException.InnerException;
            Assert.IsTrue(exception.Message.Contains("ObjectMember"));
        }

        [Test]
        public void ThrowWhenGeneratingObjectType_ThrowsNoExceptionIfTreeContainsNoObjectType()
        {
            _builder.ThrowWhenGeneratingObjectType();
            _builder.Build<TopLevel>();
        }
    }
}


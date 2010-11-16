using System;
using NUnit.Framework;
using NGineer.Exceptions;
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
            var exception = Assert.Throws<BuilderException>(() => _builder.Build<ClassWithObject>());
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


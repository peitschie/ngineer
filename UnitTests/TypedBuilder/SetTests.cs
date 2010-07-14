
using System;
using NUnit.Framework;
using NGineer.UnitTests.BuilderTests;
using Moq;
using System.Linq.Expressions;

namespace NGineer.UnitTests.TypedBuilder
{
    [TestFixture]
    public class SetTests
    {
        private ITypedBuilder<SimpleClass> _builder;
        private Mock<IBuilder> _builderMock;

        [SetUp]
        public void SetUp()
        {
            _builderMock = new Mock<IBuilder>();
            _builder = new TypedBuilder<SimpleClass>(_builderMock.Object);
        }

        [Test]
        public void SetExpression_SetsAppropriateExpression()
        {
            _builder.Set(c => c.IntField, 10);
            _builder.Build<SimpleClass>();
            _builderMock.Verify(m => m.Build(typeof(SimpleClass)));
        }
    }
}

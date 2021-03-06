
using System;
using NUnit.Framework;
using NGineer.UnitTests.BuilderTests;
using Moq;
using System.Linq.Expressions;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.TypedBuilder
{
    [TestFixture]
    public class SetTests
    {
        private ITypedBuilder<SimpleClass> _builder;
        private Mock<IConfiguredBuilder> _builderMock;

        [SetUp]
        public void SetUp()
        {
            _builderMock = new Mock<IConfiguredBuilder>();
            _builder = new TypedBuilder<SimpleClass>(_builderMock.Object, false);
        }

        [Test]
        public void SetExpression_SetsAppropriateExpression()
        {
            _builder.Set(c => c.IntField, 10);
            _builder.Build<SimpleClass>();
            _builderMock.Verify(m => m.Build<SimpleClass>());
        }
    }
}

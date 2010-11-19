using System;
using System.Collections.Generic;
using Moq;
using NGineer.Internal;
using NGineer.Utils;
using NUnit.Framework;

namespace NGineer.UnitTests.Internal
{
    [TestFixture]
    public class DefaultMemberSetterTests
    {
        private DefaultMemberSetter _setter;
        private Mock<IConfiguredBuilder> _builder;
        private BuildSession _session;
        private Random _random;

        [SetUp]
        public void SetUp()
        {
            _random = new Random(0);
            _setter = new DefaultMemberSetter();
            _builder = new Mock<IConfiguredBuilder>();
            _session = new BuildSession(_builder.Object, null, _random);
        }

        [Test]
        public void IsForMember_IndexedProperty_ReturnsFalse()
        {
            var indexedMember = typeof(IList<int>).GetMember("Item")[0];
            Assert.IsNotNull(indexedMember);
            Assert.IsFalse(_setter.IsForMember(indexedMember, null, _session));
        }
    }
}
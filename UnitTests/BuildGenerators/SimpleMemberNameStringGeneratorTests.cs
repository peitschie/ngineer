using System;
using Moq;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NGineer.Utils;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildGenerators
{
    [TestFixture]
    public class SimpleMemberNameStringGeneratorTests : GeneratorTestFixture<SimpleMemberNameStringGenerator>
    {
        protected override Type[] SupportedTypes()
        {
            return new[] {typeof (string)};
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[] {typeof (int), typeof (object), typeof (DateTime)};
        }

        [Test]
        public void Create_SimpleType_NamedMember()
        {
            var session = new BuildSession(null, null, new Random(0));
            var obj = new SimpleType();

            session.PushObject(new ObjectBuildRecord(typeof(SimpleType), obj, true));
            session.PushMember(MemberExpressions.GetMemberInfo<SimpleType>(c => c.NamedMember));

            Assert.AreEqual("NamedMember0", Generator.CreateRecord(typeof (string), null, session).Object);
            Assert.AreEqual("NamedMember1", Generator.CreateRecord(typeof (string), null, session).Object);
            Assert.AreEqual("NamedMember2", Generator.CreateRecord(typeof (string), null, session).Object);
        }

#pragma warning disable 649
        private class SimpleType
        {
            public string NamedMember { get; set; }
            public string NamedField;
            public int? SomeNumber;
        }
#pragma warning restore 649
    }
}
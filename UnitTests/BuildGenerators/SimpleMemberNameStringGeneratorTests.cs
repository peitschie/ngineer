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
            var session = new BuildSession(null, null, null, null, (Random)null);
            var obj = new SimpleType();

            session.PushObject(typeof(SimpleType), obj);
            session.PushMember(MemberExpressions.GetMemberInfo<SimpleType>(c => c.NamedMember));

            Assert.AreEqual("NamedMember0", Generator.Create(typeof (string), null, session));
            Assert.AreEqual("NamedMember1", Generator.Create(typeof (string), null, session));
            Assert.AreEqual("NamedMember2", Generator.Create(typeof (string), null, session));
        }

        [Test]
        public void DefaultConstructorGenerator_ProperlyFillsOutNamedMember()
        {
            var session = new BuildSession(null, null, null, null, (Random)null);
            var builder = new Mock<IBuilder>();
            builder.Setup(b => b.Build(typeof (string), It.IsAny<BuildSession>()))
                .Returns<Type, BuildSession>((t, s) => Generator.Create(typeof (string), builder.Object, s));

            var defaultConstructorGen = new DefaultConstructorGenerator();
            var simpleType = (SimpleType)defaultConstructorGen.Create(typeof (SimpleType), builder.Object, session);
            session.PushObject(typeof(SimpleType), simpleType);
            defaultConstructorGen.Populate(typeof(SimpleType), simpleType, builder.Object, session);
            
            Assert.IsNotNull(simpleType);
            Assert.IsNull(simpleType.SomeNumber);
            Assert.AreEqual("NamedMember0", simpleType.NamedMember);
            Assert.AreEqual("NamedField0", simpleType.NamedField);
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
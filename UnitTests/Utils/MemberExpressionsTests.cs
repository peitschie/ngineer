using System;
using System.Reflection;
using NGineer.Utils;
using NUnit.Framework;

namespace NGineer.UnitTests.Utils
{
    [TestFixture]
    public class MemberExpressionsTests
    {
		[Test]
        public void ClassParent_SimpleProperty1()
        {
            var info = (PropertyInfo)MemberExpressions.GetMemberInfo<ClassParent>(c => c.Property1);
			Assert.IsNotNull(info);
            Assert.AreEqual(typeof(ClassParent), info.ReflectedType);
            Assert.AreEqual("Property1", info.Name);
        }

		[Test]
        public void ClassParent_SimpleField1()
        {
            var info = (FieldInfo)MemberExpressions.GetMemberInfo<ClassParent>(c => c.Field1);
			Assert.IsNotNull(info);
            Assert.AreEqual(typeof(ClassParent), info.ReflectedType);
            Assert.AreEqual("Field1", info.Name);
        }
		
        [Test]
        public void InheritedMember_ReturnsInheritingClassType()
        {
            var instance = new ClassChild1();
            const int number = 10;

            var info = (PropertyInfo)MemberExpressions.GetMemberInfo<ClassChild1>(c => c.Property1);
            Assert.AreEqual(typeof(ClassChild1), info.ReflectedType);
            info.SetValue(instance, number, null);
            Assert.AreEqual(number, instance.Property1);
        }

        [Test]
        public void InheritedMember_NewProperty_HidesInherited()
        {
            var instance = new SpecificInterface();
            const int number = 10;

            var info = (PropertyInfo)MemberExpressions.GetMemberInfo<ISpecificInterface>(c => c.Property1);

            Assert.AreEqual(typeof(ISpecificInterface), info.ReflectedType);
            Assert.AreEqual(typeof(int), info.PropertyType);

            info.SetValue(instance, number, null);
            Assert.AreEqual(number, instance.Property1);
        }

        [Test]
        public void InheritedMember_NewProperty_BaseStillAccessible()
        {
            var instance = new SpecificInterface();
            const int number = 10;

            var info = (PropertyInfo)MemberExpressions.GetMemberInfo<IGeneralInterface>(c => c.Property1);

            Assert.AreEqual(typeof(IGeneralInterface), info.ReflectedType);
            Assert.AreEqual(typeof(object), info.PropertyType);

            info.SetValue(instance, number, null);
            Assert.AreEqual(number, instance.Property1);
        }

        [Test]
        public void LambdaExpression_Basic()
        {
            var info = (PropertyInfo)MemberExpressions.GetMemberInfo((ClassParent c) => c.Property1);

            Assert.AreEqual(typeof(ClassParent), info.ReflectedType);
            Assert.AreEqual(typeof(int), info.PropertyType);
        }

        public class ClassParent
        {
            public int Property1 { get; set; }
			public int Field1;
        }

        public class ClassChild1 : ClassParent { }

        public interface IGeneralInterface
        {
            object Property1 { get; set; }
        }

        public interface ISpecificInterface : IGeneralInterface
        {
            new int Property1 { get; set; }
        }

        public class SpecificInterface : ISpecificInterface
        {
            object IGeneralInterface.Property1
            {
                get { return Property1; }
                set { Property1 = (int)value; }
            }

            public int Property1 { get; set; }
        }
    }
}
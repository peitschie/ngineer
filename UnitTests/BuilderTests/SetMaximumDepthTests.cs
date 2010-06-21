using System;
using System.Collections.Generic;
using System.Text;
using NGineer.BuildHelpers;
using NGineer.Utils;
using NUnit.Framework;

namespace NGineer.UnitTests.BuilderTests
{
    [TestFixture]
    public class SetMaximumDepthTests
    {
        [Test]
        public void Build_MaximumRecursionLevel_Settable()
        {
            var newClass = new Builder(1).SetMaximumDepth(3).Build<RecursiveClass>();

            Assert.IsNotNull(newClass.RecursiveReference);
            Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference);
            Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference);
            Assert.IsNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference.RecursiveReference);
        }

        [Test]
        public void DefaultValue_ThrowsExceptionWhenReached()
        {
            var exception = Assert.Throws<BuilderDepthExceededException>(() => new Builder(1).Build<RecursiveClass>());
            Assert.AreEqual("Maximum build depth of 5 was exceeded: {0}".With(CreateString(typeof(RecursiveClass), 5)), exception.Message);
        }

        [Test]
        public void SetMaximumDepth_ManuallySet_CanThrowExceptionWhenReached()
        {
            var exception = Assert.Throws<BuilderDepthExceededException>(() => new Builder(1)
                .SetMaximumDepth(10)
                .ThrowsWhenMaximumDepthReached()
                .Build<RecursiveClass>());
            Assert.AreEqual("Maximum build depth of 10 was exceeded: {0}".With(CreateString(typeof(RecursiveClass), 10)), exception.Message);
        }

        [Test]
        public void Build_MaximumRecursionLevel_ChildContainer_SetValueBuilderEnforced()
        {
            var sessions = new List<BuildSession>();
            var newClass = new Builder(1)
                .SetMaximumDepth(2)
                .AfterPopulationOf<TestClassFourDeep>((t, b, s) =>
                {
                    sessions.Add(s);
                    t.PropertyTestClass = b.CreateNew()
                        .AfterPopulationOf<TestClassThreeDeep>((t1, b1, s1) =>
                        {
                            sessions.Add(s1);
                            t1.PropertyTestClass = b1.CreateNew()
                                .AfterPopulationOf<TestClass>((t2, b2, s2) =>
                                {
                                    sessions.Add(s2);
                                    t2.Property2 = b2.CreateNew().Build<TestClass2>(s2);
                                })
                                .Build<TestClass>(s1);
                        })
                        .Build<TestClassThreeDeep>(s);
                })
                .Build<TestClassFourDeep>();

            Assert.IsNotNull(newClass.PropertyTestClass);
            Assert.IsNotNull(newClass.PropertyTestClass.PropertyTestClass);
            Assert.IsNull(newClass.PropertyTestClass.PropertyTestClass.Property2);
        }

        [Test]
        public void Build_MaximumRecursionLevel_SetValueBuilderEnforced()
        {
            List<int> buildDepth = new List<int>();
            var newClass = new Builder(1)
                .SetMaximumDepth(2)
                .AfterPopulationOf<TestClassFourDeep>((type, b, s) =>
                {
                    buildDepth.Add(s.BuildDepth);
                    type.PropertyTestClass = b
                        .CreateNew()
                        .AfterPopulationOf<TestClassThreeDeep>((type1, b1, s1) =>
                        {
                            buildDepth.Add(s1.BuildDepth);
                            type1.PropertyTestClass = b1
                                .CreateNew()
                                .AfterPopulationOf<TestClass>((type2, b2, s2) =>
                                {
                                    buildDepth.Add(s2.BuildDepth);
                                    type2.Property2 = b2.Build<TestClass2>(s2);
                                })
                                .Build<TestClass>(s1);
                        })
                        .Build<TestClassThreeDeep>(s);
                })
                .Build<TestClassFourDeep>();

            Assert.AreEqual(new int[] { 0, 1, 2 }, buildDepth.ToArray());
            Assert.IsNotNull(newClass.PropertyTestClass);
            Assert.IsNotNull(newClass.PropertyTestClass.PropertyTestClass);
            Assert.IsNull(newClass.PropertyTestClass.PropertyTestClass.Property2);
        }

        [Test]
        public void Hierarchy_BuildDepthInheritedFromParent()
        {
            var builder = new Builder(1);
            var builder2 = builder.CreateNew();
            builder.SetMaximumDepth(3);
            Assert.AreEqual(builder.BuildDepth, builder2.BuildDepth);
        }

        [Test]
        public void Hierarchy_ChildCanBeOverridden()
        {
            var builder = new Builder(1);
            var builder2 = builder.CreateNew();
            builder.SetMaximumDepth(3);
            builder2.SetMaximumDepth(5);

            Assert.AreNotEqual(builder.BuildDepth, builder2.BuildDepth);
            Assert.AreEqual(3, builder.BuildDepth);
            Assert.AreEqual(5, builder2.BuildDepth);
        }

        private static string CreateString(Type type, int count)
        {
            var builder = new StringBuilder();
            builder.Append(type.Name);
            for (int i = 1; i <= count; i++)
            {
                builder.Insert(0, "->");
                builder.Insert(0, type.Name);
            }
            return builder.ToString();
        }
    }
}
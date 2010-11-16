using System;
using NUnit.Framework;
using NGineer.UnitTests.Types;
namespace NGineer.UnitTests.BuilderTests
{
    [TestFixture]
    public class PostBuildTests
    {
        private IBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new Builder();
        }

        [Test]
        public void PostBuild_CalledOnlyOnceAfterBuild()
        {
            var calls = 0;
            _builder.PostBuild(o => calls++);
            _builder.Build<int[]>();
            Assert.AreEqual(1, calls);
        }



        [Test]
        public void PostBuild_ParentCalledBeforeChild()
        {
            var parent = 0;
            var child = 0;
            var calls = 0;
            _builder.PostBuild(o => parent = calls++)
                 .CreateNew().PostBuild(o => child = calls++).Build<int[]>();
            Assert.AreEqual(2, calls);
            Assert.AreEqual(0, parent);
            Assert.AreEqual(1, child);
        }

        [Test]
        public void SetAfterBuild_IgnoredDuringBuild_CalledOncePerEntityAfterBuild()
        {
            var calls = 0;
            _builder.SetCollectionSize<CountsPropertySets>(4, 4)
                .For<CountsPropertySets>()
                    .Ignore(x => x.RecursiveProperty)
                    .SetAfterBuild(x => x.SomeProperty, (session) => calls++);
            var classes = _builder.Build<CountsPropertySets[]>();
            Assert.AreEqual(4, calls);
            var count = 0;
            foreach(var entry in classes)
            {
                Assert.AreEqual(1, entry.GetSomePropertySets());
                Assert.AreEqual(count++, entry.SomeProperty);
            }
        }

        [Test]
        public void SetAfterBuild_IgnoredDuringBuild_CalledOncePerEntityAfterBuild_ObjInstance()
        {
            var calls = 0;
            _builder.SetCollectionSize<CountsPropertySets>(4, 4)
                .For<CountsPropertySets>()
                    .Ignore(x => x.RecursiveProperty)
                    .SetAfterBuild(x => x.SomeProperty, (obj, session) => { calls++; return obj.GetHashCode();});
            var classes = _builder.Build<CountsPropertySets[]>();
            Assert.AreEqual(4, calls);
            foreach(var entry in classes)
            {
                Assert.AreEqual(1, entry.GetSomePropertySets());
                Assert.AreEqual(entry.GetHashCode(), entry.SomeProperty);
            }
        }
    }
}


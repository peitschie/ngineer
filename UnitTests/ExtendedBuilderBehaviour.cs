using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NGineer.UnitTests
{
    public class ExtendedBuilderBehaviour
    {
        [Test]
        public void Builder_LeavesNoTypeUnconstructed()
        {

            var newClass = new Builder(1)
                .AfterConstructionOf<Slide>(c => c.ParentCase, 
                    (o, b, s) => s.ConstructedObjects.Last(obj => typeof(Case).Equals(obj.GetType())))
                .Build<Case>();
            Assert.IsNotNull(newClass);
            foreach (var slide in newClass.Slides)
            {
                Assert.AreSame(newClass, slide.ParentCase);
            }
        }

        public class Case
        {
            public IList<Slide> Slides { get; set; }
        }

        public class Slide
        {
            public Case ParentCase { get; set; }
        }
    }
}
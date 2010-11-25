using System;
namespace NGineer.UnitTests
{
    public class SingleReadonlyClass
    {
        public int GetOnlyProperty { get { return 0; } }

        public readonly int ReadonlyField = 13;
        #pragma warning disable 0414
        private int PrivateField = 13;
        #pragma warning restore 0414

        public const int ConstantField = 10;
    }
}


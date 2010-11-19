using System.Collections.Generic;
using System;
using Moq;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NUnit.Framework;
using Range = NGineer.Internal.Range;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.BuildGenerators
{
    [TestFixture]
    public class DictionaryGeneratorTests : GeneratorTestFixture<DictionaryGenerator>
    {
        protected override Type[] SupportedTypes()
        {
            return new[]
                {
                    typeof (IDictionary<int, string>),
                    typeof (Dictionary<string, string>),
                };
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[]
                {
                    typeof(CustomDictionaryType),
                    typeof(CustomDictionaryGeneric<string>),
                    typeof (string[]),
                    typeof (ListGeneratorTests),
                    typeof (string),
                    typeof (IEnumerable<KeyValuePair<string, string>>),
                };
        }

        public class CustomDictionaryType : Dictionary<string, string> {}

        public class CustomDictionaryGeneric<TType> : Dictionary<string, TType> {}
    }
}
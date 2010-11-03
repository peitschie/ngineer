using NGineer.BuildGenerators;
using NGineer.Utils;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Reflection;

namespace NGineer
{
    public static class Generators
    {
        public static IGenerator ReuseOnly<TType>(IEnumerable<TType> collection)
        {
            return new ReuseInstancesGenerator<TType>(collection);
        }

        public static IGenerator Default<TType>()
        {
            return new DefaultConstructorGenerator(typeof (TType));
        }
    }
}
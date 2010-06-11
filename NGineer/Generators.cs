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
        public static IGenerator NullGenerator<TType>() where TType : class
        {
            return new NullObjectGenerator<TType>();
        }

        public static IGenerator ReuseOnly<TType>(IEnumerable<TType> collection)
        {
            return new ReuseInstancesGenerator<TType>(collection);
        }

        public static IGenerator Default<TType>()
        {
            return new DefaultConstructorGenerator(typeof (TType));
        }

        public static class UniqueCollection
        {
            public static IGenerator ForEnumerable<TType>(IEnumerable<TType> entries)
            {
                return new UniqueCollectionGeneratorEnumerable<TType>(entries);
            }
    
            public static IGenerator ForMember<TClassType>(Expression<Func<TClassType, object>> expression)
            {
                var memberInfo = MemberExpressions.GetMemberInfo(expression);
                var generatorType = typeof(UniqueCollectionGeneratorMember<,>)
                    .MakeGenericType(typeof(TClassType), memberInfo.ReturnType());
                var instance = generatorType.GetConstructor(new []{typeof(MemberInfo)});
                return (IGenerator)instance.Invoke(new object[]{memberInfo});
            }

            public static IGenerator ForMember<TClassType, TType>(Expression<Func<TClassType, TType>> expression)
            {
                return new UniqueCollectionGeneratorMember<TClassType, TType>(expression);
            }
        }
    }
}
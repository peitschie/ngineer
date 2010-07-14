
using System;
using NGineer.BuildHelpers;
using NGineer.Utils;
using System.Reflection;
using System.Linq.Expressions;
using NGineer.BuildGenerators;

namespace NGineer
{
    public static class IBuilderExtensions
    {
        public static IBuilder WithGenerator(this IBuilder builder, Type type, Func<IBuilder, BuildSession, object> generator)
        {
            if(generator == null)
                throw new ArgumentNullException("generator");
            return builder.WithGenerator(new BuilderGenerator(type, generator));
        }

        public static IBuilder WithGenerator<TType>(this IBuilder builder, Func<IBuilder, BuildSession, TType> generator)
        {
            if(generator == null)
                throw new ArgumentNullException("generator");
            return builder.WithGenerator(new BuilderGenerator<TType>(generator));
        }        
        
        public static IBuilder WithGenerator<TType>(this IBuilder builder, Func<TType> generator)
        {
            if(generator == null)
                throw new ArgumentNullException("generator");
            return builder.WithGenerator(new BuilderGenerator<TType>((b, s) => generator()));
        }

        public static IBuilder AfterPopulationOf<TType>(this IBuilder builder, Action<TType> setter)
        {
            if(setter == null)
                throw new ArgumentNullException("setter");
            return builder.AfterPopulationOf(new Setter<TType>(setter));
        }

        public static IBuilder AfterPopulationOf<TType>(this IBuilder builder, Action<TType, IBuilder, BuildSession> setter)
        {
            if(setter == null)
                throw new ArgumentNullException("setter");
            return builder.AfterPopulationOf(new Setter<TType>(setter));
        }

        public static IBuilder AfterConstructionOf(this IBuilder builder, MemberInfo member, Func<object, IBuilder, BuildSession, object> setter)
        {
            if(member == null)
                throw new ArgumentNullException("member");
            if(setter == null)
                throw new ArgumentNullException("setter");
            ValidateMember(member, setter);
            return builder.AfterConstructionOf(new MemberSetter(member, setter));
        }

        public static IBuilder AfterConstructionOf<TType, TReturnType>(this IBuilder builder, Expression<Func<TType,
                                                                TReturnType>> expression,
                                                                Func<TType, IBuilder, BuildSession, TReturnType> setter)
        {
            if(expression == null)
                throw new ArgumentNullException("expression");
            if(setter == null)
                throw new ArgumentNullException("setter");
            var member = MemberExpressions.GetMemberInfo(expression);
            ValidateMember(member, setter);
            return builder.AfterConstructionOf(new MemberSetter<TType, TReturnType>(member, setter));
        }
       
        public static IBuilder AfterConstructionOf<TType, TReturnType>(this IBuilder builder,
                                                                Expression<Func<TType, TReturnType>> expression,
                                                                TReturnType value)
        {
            return builder.AfterConstructionOf(expression, (o, b, s) => value);
        }

        public static IBuilder AfterConstructionOf<TType>(this IBuilder builder, Expression<Func<TType, object>> expression,
                                                                IGenerator generator)
        {
            if(expression == null)
                throw new ArgumentNullException("expression");
            if(generator == null)
                throw new ArgumentNullException("generator");
            var member = MemberExpressions.GetMemberInfo(expression);
            // No validation can be done here as the generator only returns a generic object type
            return builder.AfterConstructionOf(new GeneratorMemberSetter(member, generator));
        }

        private static void ValidateMember<TType, TReturnType>(MemberInfo member,
                                                               Func<TType, IBuilder, BuildSession, TReturnType> setter)
        {
            if(!member.ReturnType().IsAssignableFrom(typeof(TReturnType)))
            {
                throw new InvalidCastException("Unable to cast from {0} to {1}".With(typeof(TReturnType), member.ReturnType()));
            }
        }

        public static IBuilder SetCollectionSize<TType> (this IBuilder builder, int minimum, int maximum)
        {
            return builder.SetCollectionSize(typeof(TType), minimum, maximum);
        }

        public static IBuilder SetNumberOfInstances<TType> (this IBuilder builder, int minimum, int maximum)
        {
            return builder.SetNumberOfInstances(typeof(TType), minimum, maximum);
        }

        public static TType Build<TType>(this IBuilder builder)
        {
            return (TType)builder.Build(typeof(TType));
        }

        public static TType Build<TType>(this IBuilder builder, BuildSession session)
        {
            return (TType)builder.Build(typeof (TType), session);
        }

    }
}

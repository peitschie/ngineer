using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer
{
    public class TypedBuilder<TTarget> : ITypedBuilder<TTarget>
    {
        private readonly IConfiguredBuilder _parent;
        private readonly bool _allowInherited;
        private bool _isParentHooked = false;
        private Action<TTarget, BuildSession> _postbuildHook = null;

        public TypedBuilder(IConfiguredBuilder parent, bool allowInherited)
        {
            _parent = parent;
            _allowInherited = allowInherited;
        }

        private void AddAction(Action<TTarget, BuildSession> hook)
        {
            if(!_isParentHooked)
            {
                _parent.PostBuild(o =>
                {
                    foreach(var entry in o.ConstructedNodes.Select(e => e.Object).OfType<TTarget>().Distinct())
                    {
                        _postbuildHook(entry, o);
                    }
                });
                _isParentHooked = true;
            }
            _postbuildHook += hook;
        }

        #region ITypedBuilder<TTarget> implementation
        public ITypedBuilder<TTarget> Set<TReturnType>(Expression<Func<TTarget, TReturnType>> expression, Func<TTarget, IBuilder, BuildSession, TReturnType> value)
        {
            if(expression == null)
                throw new ArgumentNullException("expression");
            var member = MemberExpressions.GetMemberInfo(expression);
            ValidateMember(member, value);
            _parent.AfterConstructionOf(BuilderUtils.GetMemberSetter(member, value, _allowInherited));
            return this;
        }


        public ITypedBuilder<TTarget> Set<TReturnType>(Expression<Func<TTarget, TReturnType>> expression, TReturnType value)
        {
            Set(expression, (obj, builder, session) => value);
            return this;
        }


        public ITypedBuilder<TTarget> Set<TReturnType>(Expression<Func<TTarget, TReturnType>> expression, Func<TReturnType> value)
        {
            if(value == null)
                throw new ArgumentNullException("value");
            Set(expression, (obj, builder, session) => value());
            return this;
        }


        public ITypedBuilder<TTarget> Set(Expression<Func<TTarget, object>> expression, IGenerator generator)
        {
            _parent.AfterConstructionOf<TTarget>(expression, generator);
            return this;
        }


        public ITypedBuilder<TTarget> SetAfterBuild<TReturnType>(Expression<Func<TTarget, TReturnType>> expression, Func<BuildSession, TReturnType> value)
        {
            return SetAfterBuild(expression, (obj, session) => value(session));
        }


        public ITypedBuilder<TTarget> SetAfterBuild<TReturnType>(Expression<Func<TTarget, TReturnType>> expression, Func<TTarget, BuildSession, TReturnType> value)
        {
            Ignore(expression);
            var accessor = expression.GetMemberInfo();
            AddAction((obj, session) => { accessor.SetValue(obj, value(obj, session)); });
            return this;
        }

        public ITypedBuilder<TTarget> Ignore<TReturnType>(Expression<Func<TTarget, TReturnType>> expression)
        {
            _parent.IgnoreMember(MemberExpressions.GetMemberInfo(expression), _allowInherited);
            return this;
        }

        public ITypedBuilder<TTarget> Ignore(Expression<Func<TTarget, object>> expression)
        {
            _parent.IgnoreMember(MemberExpressions.GetMemberInfo(expression), _allowInherited);
            return this;
        }

        public ITypedBuilder<TTarget> IgnoreUnset()
        {
            _parent.IgnoreUnset(typeof(TTarget));
            return this;
        }

        public ITypedBuilder<TTarget> IgnoreAll()
        {
            foreach(var member in typeof(TTarget).GetMembers().Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property))
            {
                _parent.IgnoreMember(member, false);
            }
            return this;
        }

        public ITypedBuilder<TTarget> Do(Action<TTarget> setter)
        {
            if(setter == null)
                throw new ArgumentNullException("setter");
            _parent.AfterPopulationOf(BuilderUtils.GetSetter<TTarget>((obj, buildr, session) =>
            {
                setter(obj);
                return obj;
            }, _allowInherited));
            return this;
        }


        public ITypedBuilder<TTarget> Do(Action<TTarget, IBuilder, BuildSession> setter)
        {
            if(setter == null)
                throw new ArgumentNullException("setter");
            _parent.AfterPopulationOf(BuilderUtils.GetSetter<TTarget>((obj, buildr, session) =>
            {
                setter(obj, buildr, session);
                return obj;
            }, _allowInherited));
            return this;
        }

        #endregion

        #region IBuilder implementation
        public object Build(Type type)
        {
            return _parent.Build(type);
        }


        public object Build(Type type, BuildSession session)
        {
            return _parent.Build(type, session);
        }


        public TType Build<TType>()
        {
            return _parent.Build<TType>();
        }


        public TType Build<TType>(BuildSession session)
        {
            return _parent.Build<TType>(session);
        }


        public IBuilder SetMaximumDepth(Nullable<int> depth)
        {
            return _parent.SetMaximumDepth(depth);
        }


        public IBuilder SetMaximumObjects(Nullable<int> maximum)
        {
            return _parent.SetMaximumObjects(maximum);
        }


        public IBuilder ThrowsWhenMaximumDepthReached()
        {
            return _parent.ThrowsWhenMaximumDepthReached();
        }


        public IBuilder WithGenerator(IGenerator generator)
        {
            return _parent.WithGenerator(generator);
        }

        public IBuilder WithGenerator(Type type, Func<IBuilder, BuildSession, object> generator)
        {
            return _parent.WithGenerator(type, generator);
        }

        public IBuilder WithGenerator<TType>(Func<IBuilder, BuildSession, TType> generator)
        {
            return _parent.WithGenerator(generator);
        }

        public IBuilder WithGenerator<TType>(Func<TType> generator)
        {
            return _parent.WithGenerator(generator);
        }


        public IBuilder AfterConstructionOf(IMemberSetter setter)
        {
            return _parent.AfterConstructionOf(setter);
        }

        public IBuilder IgnoreUnset(Type type)
        {
            return _parent.IgnoreUnset(type);
        }

        public IBuilder AfterPopulationOf(IInstanceProcessor setter)
        {
            return _parent.AfterPopulationOf(setter);
        }


        public IBuilder SetDefaultCollectionSize(int minimum, int maximum)
        {
            return _parent.SetDefaultCollectionSize(minimum, maximum);
        }


        public IBuilder SetCollectionSize(Type type, int minimum, int maximum)
        {
            return _parent.SetCollectionSize(type, minimum, maximum);
        }

        public IBuilder SetCollectionSize<TType>(int minimum, int maximum)
        {
            return _parent.SetCollectionSize(typeof(TType), minimum, maximum);
        }

        public IBuilder SetNumberOfInstances(Type type, int minimum, int maximum)
        {
            return _parent.SetNumberOfInstances(type, minimum, maximum);
        }

        public IBuilder SetNumberOfInstances<TType>(int minimum, int maximum)
        {
            return _parent.SetNumberOfInstances<TType>(minimum, maximum);
        }

        public ITypedBuilder<TType> For<TType>()
        {
            return _parent.For<TType>();
        }

        public ITypedBuilder<TType> For<TType>(bool allowInherited)
        {
            return _parent.For<TType>(allowInherited);
        }


        public IBuilder PostBuild(Action<BuildSession> hook)
        {
            return _parent.PostBuild(hook);
        }


        public IBuilder CreateNew()
        {
            return _parent.CreateNew();
        }

        public IBuilder CreateNew(BuildSession session)
        {
            return _parent.CreateNew(session);
        }


        public IBuilder Sealed()
        {
            return _parent.Sealed();
        }


        public int BuildDepth
        {
            get { return _parent.BuildDepth; }
        }

        #endregion

        private static void ValidateMember<TType, TReturnType>(MemberInfo member, Func<TType, IBuilder, BuildSession, TReturnType> setter)
        {
            if(!member.ReturnType().IsAssignableFrom(typeof(TReturnType)))
            {
                throw new InvalidCastException(string.Format("Unable to cast from {0} to {1}", typeof(TReturnType), member.ReturnType()));
            }
        }
    }
}

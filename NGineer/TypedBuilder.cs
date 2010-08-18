using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer
{
    public class TypedBuilder<TTarget> : ITypedBuilder<TTarget>
    {
        private readonly IBuilder _parent;

        public TypedBuilder (IBuilder parent)
        {
            _parent = parent;
        }

        #region ITypedBuilder<TTarget> implementation
        public ITypedBuilder<TTarget> Set<TReturnType> (Expression<Func<TTarget, TReturnType>> expression, Func<TTarget, IBuilder, BuildSession, TReturnType> value)
        {
            _parent.AfterConstructionOf<TTarget, TReturnType>(expression, value);
            return this;
        }


        public ITypedBuilder<TTarget> Set<TReturnType> (Expression<Func<TTarget, TReturnType>> expression, TReturnType value)
        {
            _parent.AfterConstructionOf<TTarget, TReturnType>(expression, value);
            return this;
        }
        
        
        public ITypedBuilder<TTarget> Set (Expression<Func<TTarget, object>> expression, IGenerator generator)
        {
            _parent.AfterConstructionOf<TTarget>(expression, generator);
            return this;
        }
        
        
        public ITypedBuilder<TTarget> Ignore (Expression<Func<TTarget, object>> expression)
        {
            _parent.IgnoreMember<TTarget>(expression);
            return this;
        }

        public ITypedBuilder<TTarget> IgnoreUnset()
        {
            _parent.IgnoreUnset(typeof(TTarget));
            return this;
        }

        public ITypedBuilder<TTarget> IgnoreAll()
        {
        	foreach (var member in typeof(TTarget).GetMembers()
				.Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)) 
			{
				_parent.IgnoreMember(member);
			}
            return this;
        }

        public ITypedBuilder<TTarget> Do (Action<TTarget> setter)
        {
            _parent.AfterPopulationOf<TTarget>(setter);
            return this;
        }


        public ITypedBuilder<TTarget> Do (Action<TTarget, IBuilder, BuildSession> setter)
        {
            _parent.AfterPopulationOf<TTarget>(setter);
            return this;
        }
        
        #endregion

        #region IBuilder implementation
        public object Build (Type type)
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

		
        public IBuilder SetMaximumDepth (Nullable<int> depth)
        {
            return _parent.SetMaximumDepth(depth);
        }


        public IBuilder SetMaximumObjects (Nullable<int> maximum)
        {
            return _parent.SetMaximumObjects(maximum);
        }
        
        
        public IBuilder ThrowsWhenMaximumDepthReached ()
        {
            return _parent.ThrowsWhenMaximumDepthReached();
        }
        
        
        public IBuilder WithGenerator (IGenerator generator)
        {
            return _parent.WithGenerator(generator);
        }

        
        public IBuilder AfterConstructionOf (IMemberSetter setter)
        {
            return _parent.AfterConstructionOf(setter);
        }

        public IBuilder IgnoreMember(MemberInfo member)
        {
            return _parent.IgnoreMember(member);
        }

        public IBuilder IgnoreUnset(Type type)
        {
            return _parent.IgnoreUnset(type);
        }

        public IBuilder AfterPopulationOf (ISetter setter)
        {
            return _parent.AfterPopulationOf(setter);
        }


        public IBuilder SetDefaultCollectionSize (int minimum, int maximum)
        {
            return _parent.SetDefaultCollectionSize(minimum, maximum);
        }


        public IBuilder SetCollectionSize (Type type, int minimum, int maximum)
        {
            return _parent.SetCollectionSize(type, minimum, maximum);
        }

		public IBuilder SetCollectionSize<TType>(int minimum, int maximum)
		{
			return _parent.SetCollectionSize(typeof(TType), minimum, maximum);
		}

        public IBuilder SetNumberOfInstances (Type type, int minimum, int maximum)
        {
            return _parent.SetNumberOfInstances(type, minimum, maximum);
        }
        
        
        public ITypedBuilder<TType> For<TType> ()
        
        {
            return _parent.For<TType>();
        }


        public IBuilder CreateNew ()
        {
            return _parent.CreateNew();
        }


        public IBuilder Sealed ()
        {
            return _parent.Sealed();
        }


        public int BuildDepth {
            get {
                return _parent.BuildDepth;
            }
        }
        
        #endregion
    }
}

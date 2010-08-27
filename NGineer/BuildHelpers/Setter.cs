﻿using System;
using System.Collections.Generic;

namespace NGineer.BuildHelpers
{
    public class Setter<TType> : ISetter
    {
		private readonly Func<Type, bool> _typeCheck;
        private readonly Func<TType, IBuilder, BuildSession, TType> _setter;

        public Setter(Func<TType, IBuilder, BuildSession, TType> setter, bool allowInherited)
        {
        	if (setter == null)
        		throw new ArgumentNullException("setter");
        	_setter = setter;
			if (allowInherited) 
			{
				_typeCheck = t => typeof(TType).IsAssignableFrom(t);
			} 
			else 
			{
				_typeCheck = t => typeof(TType) == t;
			}
        }

        public object Set(object obj, IBuilder builder, BuildSession session)
        {
            return _setter((TType)obj, builder, session);
        }

        public bool IsForType(Type type)
        {
            return type != null && _typeCheck(type);
        }
    }
}
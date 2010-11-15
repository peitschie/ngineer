
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace NGineer.Exceptions
{
    public class MemberExpressionException : Exception
    {
        public MemberExpressionException(Expression expression)
            : base(string.Format("Cannot interpret member from {0}", expression.NodeType))
        { }
    }
}

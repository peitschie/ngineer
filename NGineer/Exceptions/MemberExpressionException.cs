
using System;
using System.Linq.Expressions;

namespace NGineer.Exceptions
{
    public class MemberExpressionException : Exception
    {
        public MemberExpressionException(Expression expression)
            : base(string.Format("Cannot interpret member from {0}", expression.NodeType))
        { }
    }
}

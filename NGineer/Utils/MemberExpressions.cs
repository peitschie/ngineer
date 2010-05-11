using System.Linq.Expressions;
using System;
using System.Reflection;

namespace NGineer.Utils
{
	public static class MemberExpressions
	{
		public static MemberInfo GetMemberInfo(Expression expression)
        {
            if(expression is MemberExpression)
            {
				var memberExpression = (MemberExpression)expression;
				return memberExpression.Member;
            }
            if(expression is UnaryExpression)
            {
				var unaryExpression = (UnaryExpression)expression;
				if (unaryExpression.NodeType == ExpressionType.Convert)
				{
					return GetMemberInfo(unaryExpression.Operand);
				}
            }
            throw new InvalidOperationException("Cannot interpret member from {0}".With(expression.NodeType));
        }
		
		public static MemberInfo GetMemberInfo<TType>(Expression<Func<TType, object>> expression)
        {
			return GetMemberInfo(expression.Body);
        }
	}
}

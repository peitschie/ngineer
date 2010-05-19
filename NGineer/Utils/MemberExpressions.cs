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
                // Need to ensure this returns the property setter for the requested type, not
                // the type that defines the member.  I was unable to find a clean way of
                // changing the property accessor to by on the requested type.
				return memberExpression.Expression.Type.GetProperty(memberExpression.Member.Name);
            }
            if(expression is UnaryExpression)
            {
				var unaryExpression = (UnaryExpression)expression;
				if (unaryExpression.NodeType == ExpressionType.Convert)
				{
					return GetMemberInfo(unaryExpression.Operand);
				}
            }
            if(expression is LambdaExpression)
            {
                var lambdaExpression = (LambdaExpression) expression;
                return GetMemberInfo(lambdaExpression.Body);
            }
            throw new InvalidOperationException("Cannot interpret member from {0}".With(expression.NodeType));
        }
		
		public static MemberInfo GetMemberInfo<TType>(Expression<Func<TType, object>> expression)
        {
			return GetMemberInfo(expression.Body);
        }
	}
}

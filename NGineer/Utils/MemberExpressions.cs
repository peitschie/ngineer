using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.Reflection;

namespace NGineer.Utils
{
	public static class MemberExpressions
	{
		/// <summary>
		/// // http://handcraftsman.wordpress.com/2008/11/11/how-to-get-c-property-names-without-magic-strings/ 
		/// </summary>
		/// <param name="expression">
		/// A <see cref="Expression"/>
		/// </param>
		/// <returns>
		/// A <see cref="MemberInfo"/>
		/// </returns>
		public static MemberInfo GetMemberInfo(Expression expression)
        {
            if(expression is MemberExpression)
            {
				var memberExpression = (MemberExpression)expression;
                // Need to ensure this returns the property setter for the requested type, not
                // the type that defines the member.  I was unable to find a clean way of
                // changing the property accessor to by on the requested type.
                return memberExpression.Expression.Type.GetMember(memberExpression.Member.Name).FirstOrDefault();
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

        public static MemberInfo[] GetExpressionChain<TType>(Expression<Func<TType, object>> expression)
        {
            return GetExpressionChain(expression.Body);
        }

        public static MemberInfo[] GetExpressionChain(Expression expression)
        {
            var memberChain = new List<MemberInfo>();
            GetExpressionChain(expression, memberChain);
            return memberChain.ToArray();
        }

        private static void GetExpressionChain(Expression expression, List<MemberInfo> memberChain)
        {
            if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression)expression;
                if (memberExpression.Expression != null && memberExpression.Expression is MemberExpression)
                {
                    GetExpressionChain(memberExpression.Expression, memberChain);
                }
                memberChain.Add(GetMemberInfo(memberExpression));
            }
            else if (expression is UnaryExpression && expression.NodeType == ExpressionType.Convert)
            {
                var unaryExpression = (UnaryExpression)expression;
                GetExpressionChain(unaryExpression.Operand, memberChain);
            }
            else if (expression is LambdaExpression)
            {
                var lambdaExpression = (LambdaExpression)expression;
                GetExpressionChain(lambdaExpression.Body, memberChain);
            }
            else
            {
                throw new InvalidOperationException("Cannot interpret member from {0}".With(expression.NodeType));
            }
        }
	}
}

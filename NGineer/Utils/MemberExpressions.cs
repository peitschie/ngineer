using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.Exceptions;

namespace NGineer.Utils
{
    /// <summary>
    /// // http://handcraftsman.wordpress.com/2008/11/11/how-to-get-c-property-names-without-magic-strings/ 
    /// </summary>
    public static class MemberExpressions
    {
        public static MemberInfo GetMemberInfo<TMember>(this Expression<Func<TMember, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            return GetMemberInfo(expression.Body);
        }

        public static MemberInfo GetMemberInfo<TMember, TReturn>(this Expression<Func<TMember, TReturn>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            return GetMemberInfo(expression.Body);
        }

        public static MemberInfo GetMemberInfo(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                // Need to ensure this returns the property setter for the requested type, not
                // the type that defines the member.  I was unable to find a clean way of
                // changing the property accessor to by on the requested type.
                return memberExpression.Expression.Type.GetMember(memberExpression.Member.Name).FirstOrDefault();
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                return GetMemberInfo(unaryExpression.Operand);
            }

            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
            {
                return GetMemberInfo(lambdaExpression.Body);
            }
            throw new MemberExpressionException(expression);
        }

        public static MemberInfo[] GetExpressionChain<TType>(this Expression<Func<TType, object>> expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            return GetExpressionChain(expression.Body);
        }

        public static MemberInfo[] GetExpressionChain(this Expression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            var memberChain = new List<MemberInfo>();
            GetExpressionChain(expression, memberChain);
            return memberChain.ToArray();
        }

        private static void GetExpressionChain(Expression expression, List<MemberInfo> memberChain)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                if (memberExpression.Expression != null && (
                    memberExpression.Expression is MemberExpression
                    || (memberExpression.Expression is UnaryExpression && memberExpression.Expression.NodeType == ExpressionType.Convert)
                    || memberExpression.Expression is LambdaExpression))
                {
                    GetExpressionChain(memberExpression.Expression, memberChain);
                }
                memberChain.Add(GetMemberInfo(memberExpression));
                return;
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                GetExpressionChain(unaryExpression.Operand, memberChain);
                return;
            }

            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
            {
                GetExpressionChain(lambdaExpression.Body, memberChain);
                return;
            }

            throw new MemberExpressionException(expression);
        }
    }
}

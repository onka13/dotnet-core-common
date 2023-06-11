using System;
using System.Linq.Expressions;

namespace CoreCommon.Infrastructure.Helpers
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> Init<T>(Expression<Func<T, bool>> expr) { return expr; }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            ParameterExpression parameter1 = expr1.Parameters[0];
            var visitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter1);
            var body2WithParam1 = visitor.Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, body2WithParam1), parameter1);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            ParameterExpression parameter1 = expr1.Parameters[0];
            var visitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter1);
            var body2WithParam1 = visitor.Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, body2WithParam1), parameter1);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (ReferenceEquals(node, _oldParameter))
                    return _newParameter;

                return base.VisitParameter(node);
            }
        }
    }
}

using System.Linq.Expressions;

namespace CoreCommon.Infrastructure.Helpers
{
    public static class ExpressionHelper
    {
        public static Expression<TDelegate> AndAlso<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right)
        {
            return Expression.Lambda<TDelegate>(Expression.AndAlso(left, right), left.Parameters);
        }
    }
}

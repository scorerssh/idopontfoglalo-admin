using AutoFilterer;
using AutoFilterer.Attributes;
using System.Linq.Expressions;
using System.Reflection;

namespace ApartManBackend.Aditional.CoustumeFillter
{
    public class InCollectionByIdAttribute : FilteringOptionsBaseAttribute
    {
        public override Expression BuildExpression(ExpressionBuildContext context)
        {
            var filterValue = Expression.Constant(context.FilterObjectPropertyValue, typeof(int));
            var entityExpression =
                context.GetType().GetProperty("ExpressionBody", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(context) as Expression
                ?? context.GetType().GetProperty("Body", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(context) as Expression
                ?? throw new InvalidOperationException("Expression body was not found on ExpressionBuildContext.");

            var collectionExpression = Expression.Property(entityExpression, context.TargetProperty);

            var itemParameter = Expression.Parameter(context.TargetProperty.PropertyType.GenericTypeArguments[0], "a");
            var itemIdProperty = Expression.Property(itemParameter, "Id");
            var equalsExpression = Expression.Equal(itemIdProperty, filterValue);

            var anyLambda = Expression.Lambda(equalsExpression, itemParameter);
            var anyMethod = typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
                .MakeGenericMethod(itemParameter.Type);

            return Expression.Call(anyMethod, collectionExpression, anyLambda);
        }
    }
}

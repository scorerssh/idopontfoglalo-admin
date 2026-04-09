using ApartManBackend.Models.DbModels.Models;
using AutoFilterer;
using AutoFilterer.Attributes;
using System.Linq.Expressions;
using System.Reflection;

namespace ApartManBackend.Aditional.CoustumeFillter
{
    public abstract class RoomApartmanFilterAttributeBase : FilteringOptionsBaseAttribute
    {
        protected static Expression GetEntityExpression(ExpressionBuildContext context)
        {
            return context.GetType().GetProperty("ExpressionBody", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(context) as Expression
                ?? context.GetType().GetProperty("Body", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(context) as Expression
                ?? throw new InvalidOperationException("Expression body was not found on ExpressionBuildContext.");
        }

        protected static MemberExpression GetApartmanExpression(ExpressionBuildContext context)
        {
            var entityExpression = GetEntityExpression(context);
            return Expression.Property(entityExpression, context.TargetProperty);
        }
    }

    public class RoomInUserByIdAttribute : RoomApartmanFilterAttributeBase
    {
        public override Expression BuildExpression(ExpressionBuildContext context)
        {
            var apartmanExpression = GetApartmanExpression(context);
            var usersExpression = Expression.Property(apartmanExpression, nameof(Apartman.Users));
            var filterValue = Expression.Constant(context.FilterObjectPropertyValue, typeof(int));

            var userParameter = Expression.Parameter(typeof(User), "u");
            var userIdProperty = Expression.Property(userParameter, nameof(User.Id));
            var equalsExpression = Expression.Equal(userIdProperty, filterValue);

            var anyLambda = Expression.Lambda(equalsExpression, userParameter);
            var anyMethod = typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(User));

            return Expression.Call(anyMethod, usersExpression, anyLambda);
        }
    }
}

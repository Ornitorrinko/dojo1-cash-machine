using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Dojo.Tests
{
    public static class Reflection
    {
        public static void SetValue<T>(this object obj, Expression<Func<T>> property, object value)
        {
            var memberExpression = property.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("Invalid property.");
            }

            var propertyName = memberExpression.Member.Name;
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            var converted = IsConvertible(propertyInfo) ? Convert.ChangeType(value, propertyInfo.PropertyType) : value;
            propertyInfo.SetValue(obj, converted, null);
        }

        private static bool IsConvertible(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsSubclassOf(typeof(IConvertible));
        }

        public static string GetPropertyName<T>(Expression<Func<T>> property)
        {
            var me = property.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }
    }
}
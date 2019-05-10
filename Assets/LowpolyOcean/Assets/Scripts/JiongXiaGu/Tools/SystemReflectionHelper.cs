using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu
{

    public interface IReflectiveField
    {
        string Name { get; }
        Type FieldType { get; }
        MemberInfo MemberInfo { get; }
        object GetValue(object instance);
        void SetValue(object instance, object value);
    }

    public class FieldMember : IReflectiveField
    {
        private Func<object, object> getValue;
        private Action<object, object> setValue;
        public FieldInfo FieldInfo { get; private set; }
        public string Name => FieldInfo.Name;
        public Type FieldType => FieldInfo.FieldType;
        public MemberInfo MemberInfo => FieldInfo;

        public FieldMember(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException(nameof(fieldInfo));

            FieldInfo = fieldInfo;

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression value = Expression.Parameter(typeof(object), "value");
            UnaryExpression instanceCast = Expression.TypeAs(instance, fieldInfo.DeclaringType);
            var field = Expression.Field(instanceCast, fieldInfo);

            getValue = Expression.Lambda<Func<object, object>>(
                Expression.TypeAs(field, typeof(object)), instance
                ).Compile();

            setValue = Expression.Lambda<Action<object, object>>(
                Expression.Assign(field, Expression.Convert(value, fieldInfo.FieldType)), instance, value
                ).Compile();
        }

        public object GetValue(object instance)
        {
            return getValue(instance);
        }

        public void SetValue(object instance, object value)
        {
            setValue(instance, value);
        }
    }

    public class PropertyMember : IReflectiveField
    {
        private Func<object, object> getValue;
        private Action<object, object> setValue;
        public PropertyInfo PropertyInfo { get; private set; }
        public string Name => PropertyInfo.Name;
        public Type FieldType => PropertyInfo.PropertyType;
        public MemberInfo MemberInfo => PropertyInfo;

        public PropertyMember(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            PropertyInfo = propertyInfo;

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            UnaryExpression instanceCast = !propertyInfo.DeclaringType.IsValueType ? Expression.TypeAs(instance, propertyInfo.DeclaringType) : Expression.Convert(instance, propertyInfo.DeclaringType);
            getValue = Expression.Lambda<Func<object, object>>
            (
                Expression.TypeAs(Expression.Call(instanceCast, propertyInfo.GetMethod), typeof(object)), instance
            ).Compile();

            ParameterExpression value = Expression.Parameter(typeof(object), "value");
            setValue = Expression.Lambda<Action<object, object>>
            (
                Expression.Call(instanceCast, propertyInfo.SetMethod, Expression.Convert(value, propertyInfo.PropertyType)), instance, value
            ).Compile();
        }

        public object GetValue(object instance)
        {
            return getValue(instance);
        }

        public void SetValue(object instance, object value)
        {
            setValue(instance, value);
        }
    }
}

using System;
using System.Configuration;
using System.Linq;
using System.Configuration.Internal;
using System.Reflection;
using System.Collections.Generic;

namespace NConfig
{
    internal class ReflectionAccessor
    {
        private readonly Func<string, Func<Object, Object>> PropertyGetter;
        private readonly Func<string, Action<Object, Object>> PropertySetter;
        private readonly Func<string, Func<Object, Object>> FieldGetter;
        private readonly Func<string, Action<Object, Object>> FieldSetter;
        private readonly Func<string, Object[], Func<Object, Object[], Object>> MethodAccessor;


        private readonly Type accessedType;
        private readonly object instance;

        private ReflectionAccessor(Type accessedType, object instance)
        {
            PropertyGetter = (PropertyGetter = PropertyGetterFunc).Memoize();
            PropertySetter = (PropertySetter = PropertySetterAction).Memoize();
            FieldGetter = (FieldGetter = FieldGetterFunc).Memoize();
            FieldSetter = (FieldSetter = FieldSetterAction).Memoize();

            MethodAccessor = (MethodAccessor = MethodFunc).Memoize();

            this.accessedType = accessedType;
            this.instance = instance;
        }


        public ReflectionAccessor(Type accessedType) : this (accessedType, null) {}

        public ReflectionAccessor(object instance) : this (instance.GetType(), instance) {}

        public Type AccessedType
        {
            get
            {
                return accessedType;
            }
        }


        public T GetProperty<T>(string name)
        {
            // This method performance could be greatly increased by using DynamicDelegate.
            return (T)GetProperty(name);
        }

        public object GetProperty(string name)
        {
            return PropertyGetter(name)(instance);
        }

        public void SetProperty(string name, object value)
        {
            PropertySetter(name)(instance, value);
        }


        public T GetField<T>(string name)
        {
           return (T)GetField(name);
        }

        public object GetField(string name)
        {
            return FieldGetter(name)(instance);
        }

        public void SetField(string name, object value)
        {
            FieldSetter(name)(instance, value);
        }

        public object Execute(string name, params object[] args)
        {
            return MethodAccessor(name, args)(instance, args);
        }


        private Func<Object, Object[], Object> MethodFunc(string name, Object[] args)
        {
            Type[] types = null;
            if (args != null)
                types = args.Select(a => a.GetType()).ToArray();

            var mi = AccessedType.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance,
                null,
                types,
                null);
            if (mi != null)
                return (i, a) => mi.Invoke(i, a);
            return null;
        }

        private Func<object, object> FieldGetterFunc(string name)
        {
            FieldInfo fi = AccessedType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            return fi.GetValue;
        }

        private Action<object, object> FieldSetterAction(string name)
        {
            FieldInfo fi = AccessedType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            return fi.SetValue;
        }


        private Func<object,object> PropertyGetterFunc(string name)
        {
            PropertyInfo pi = AccessedType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            var getter = pi.GetGetMethod(true);
            if (getter != null)
                return o => getter.Invoke(o, null);
            return null;
        }

        private Action<object, object> PropertySetterAction(string name)
        {
            PropertyInfo pi = AccessedType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            var setter = pi.GetSetMethod(true);
            if (setter != null)
                return (i, v) => setter.Invoke(i, new Object[] {v});
            return null;
        }

    }

}

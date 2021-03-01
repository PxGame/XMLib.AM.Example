/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/4/3 17:19:34
 */

using System;
using System.Text;

namespace XMLib
{
    /// <summary>
    /// TypeUtility
    /// </summary>
    public static class TypeUtility
    {
        public static string GetSimpleName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            StringBuilder builder = new StringBuilder();

            int index = type.Name.IndexOf('`');
            builder.Append(type.Name.Remove(index));
            builder.Append('<');
            Type[] args = type.GetGenericArguments();
            for (int i = 0; i < args.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(args[i].GetSimpleName());
            }
            builder.Append('>');

            return builder.ToString();
        }

        public static object CreateInstance(Type type)
        {
            return type == typeof(string) ? string.Empty : Activator.CreateInstance(type);
        }

        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        public static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static T GetDefaultValue<T>()
        {
            return (T)GetDefaultValue(typeof(T));
        }

        #region ConvertTo Ext

        public static T ConvertTo<T>(this object from, T defaultValue)
        {
            return (T)ConvertTo(from, typeof(T), false, defaultValue);
        }

        public static object ConvertTo(this object from, Type to, object defaultValue)
        {
            return ConvertTo(from, to, false, defaultValue);
        }

        public static T ConvertAutoTo<T>(this object from)
        {
            return (T)ConvertTo(from, typeof(T), true, GetDefaultValue(typeof(T)));
        }

        public static object ConvertAutoTo(this object from, Type to)
        {
            return ConvertTo(from, to, true, GetDefaultValue(to));
        }

        #endregion ConvertTo Ext

        public static object ConvertTo(this object from, Type to, bool autoDefault, object defaultValue)
        {
            try
            {
                if (!ConvertToChecker(from.GetType(), to))
                {
                    return autoDefault ? CreateInstance(to) : defaultValue;
                }
                return ConvertTo(from, to);
            }
            catch
            {
                return autoDefault ? CreateInstance(to) : defaultValue;
            }
        }

        public static object ConvertTo(this object from, Type to)
        {
            if (to.IsEnum)
            {
                if (from is string str)
                {
                    return Enum.Parse(to, str, true);
                }
                return Enum.ToObject(to, from);
            }

            return Convert.ChangeType(from, to);
        }

        public static bool ConvertToChecker(this Type from, Type to)
        {
            if (from == null || to == null)
            {
                return false;
            }

            // 总是可以隐式类型转换为 Object。
            if (to == typeof(object))
            {
                return true;
            }

            if (to.IsAssignableFrom(from))
            {
                return true;
            }

            if (typeof(IConvertible).IsAssignableFrom(from) &&
                typeof(IConvertible).IsAssignableFrom(to))
            {
                return true;
            }

            return false;
        }

        /*
        static private bool PrimitiveConvert(Type from, Type to)
        {
            if (!(from.IsPrimitive && to.IsPrimitive))
            {
                return false;
            }

            TypeCode typeCodeFrom = Type.GetTypeCode(from);
            TypeCode typeCodeTo = Type.GetTypeCode(to);

            if (typeCodeFrom == typeCodeTo)
                return true;

            if (typeCodeFrom == TypeCode.Char)
                switch (typeCodeTo)
                {
                    case TypeCode.UInt16: return true;
                    case TypeCode.UInt32: return true;
                    case TypeCode.Int32: return true;
                    case TypeCode.UInt64: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    default: return false;
                }

            if (typeCodeFrom == TypeCode.Byte)
                switch (typeCodeTo)
                {
                    case TypeCode.Char: return true;
                    case TypeCode.UInt16: return true;
                    case TypeCode.Int16: return true;
                    case TypeCode.UInt32: return true;
                    case TypeCode.Int32: return true;
                    case TypeCode.UInt64: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            if (typeCodeFrom == TypeCode.SByte)
                switch (typeCodeTo)
                {
                    case TypeCode.Int16: return true;
                    case TypeCode.Int32: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            if (typeCodeFrom == TypeCode.UInt16)
                switch (typeCodeTo)
                {
                    case TypeCode.UInt32: return true;
                    case TypeCode.Int32: return true;
                    case TypeCode.UInt64: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            if (typeCodeFrom == TypeCode.Int16)
                switch (typeCodeTo)
                {
                    case TypeCode.Int32: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            if (typeCodeFrom == TypeCode.UInt32)
                switch (typeCodeTo)
                {
                    case TypeCode.UInt64: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            if (typeCodeFrom == TypeCode.Int32)
                switch (typeCodeTo)
                {
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            if (typeCodeFrom == TypeCode.UInt64)
                switch (typeCodeTo)
                {
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            if (typeCodeFrom == TypeCode.Int64)
                switch (typeCodeTo)
                {
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            if (typeCodeFrom == TypeCode.Single)
                switch (typeCodeTo)
                {
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }
            return false;
        }
        */
    }
}
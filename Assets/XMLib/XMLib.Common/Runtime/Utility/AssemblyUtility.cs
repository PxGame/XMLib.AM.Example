/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/1 11:11:55
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace XMLib
{
    /// <summary>
    /// AssemblyUtility
    /// </summary>
    public static class AssemblyUtility
    {
        public static void ForeachType(Action<Type> callback)
        {
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblys)
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    callback(type);
                }
            }
        }

        public static List<Type> FindAllSubclass<T>()
        {
            List<Type> results = new List<Type>();

            ForeachType((type) =>
            {
                if (!type.IsAbstract && type.IsSubclassOf(typeof(T)))
                {
                    results.Add(type);
                }
            });

            return results;
        }

        public static void ForeachTypeWithAttr<T>(Action<Type, T> callback) where T : Attribute
        {
            ForeachType((t) =>
            {
                T atrr = t.GetCustomAttribute<T>();
                if (atrr != null)
                {
                    callback(t, atrr);
                }
            });
        }

        public static List<Type> FindAllTypeWithAttr<T>() where T : Attribute
        {
            List<Type> results = new List<Type>();

            ForeachTypeWithAttr<T>((t, attr) =>
            {
                results.Add(t);
            });

            return results;
        }
    }
}
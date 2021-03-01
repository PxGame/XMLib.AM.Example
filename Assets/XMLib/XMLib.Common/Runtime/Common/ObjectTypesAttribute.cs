/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/4/2 22:25:02
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace XMLib
{
    /// <summary>
    /// ObjectTypesAttribute
    /// <para>如果目标变量是 object 类型，只能使用自己包装的类型，
    /// 一定不要直接使用Vector2，Vector3，Vector4 等Unity自建的复合类型和基础类型，
    /// 否者在json序列化的时候无法包含数据类型声明，导致反序列化后数据类型不正确</para>
    /// <para>可使用PackageData中的类型进行替代</para>
    /// <para>最好同时声明 SerializeReference 使unity能够序列化</para>
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ObjectTypesAttribute : Attribute
    {
        public Type[] types
        {
            get
            {
                HashSet<Type> list = new HashSet<Type>();
                if (baseType != null)
                {
#if UNITY_EDITOR
                    foreach (var item in UnityEditor.TypeCache.GetTypesDerivedFrom(baseType))
                    {
                        list.Add(item);
                    }
#endif
                }
                if (extraTypes != null)
                {
                    foreach (var item in extraTypes)
                    {
                        list.Add(item);
                    }
                }
                return list.ToArray();
            }
        }

        public virtual Type baseType { get; set; }
        public Type[] extraTypes { get; protected set; }

        public ObjectTypesAttribute(params Type[] extraTypes)
        {
            this.extraTypes = extraTypes;
        }
    }

    public class ObjectTypesPrimaryAttribute : ObjectTypesAttribute
    {
        public static readonly Type[] primaryTypes = {
            typeof(P<int>),
            typeof(P<float>),
            typeof(P<bool>),
            typeof(P<string>),
            typeof(P<UnityEngine.Vector2>),
            typeof(P<UnityEngine.Vector3>),
        };

        public ObjectTypesPrimaryAttribute(params Type[] extraTypes) :
            base(ArrayUtility.Combine(extraTypes, primaryTypes))
        {
        }
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/1/25 11:51:14
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// ObjectDrawer
    /// 
    /// 静态函数 object Func(GUIContent title, object obj, Type type, object[] attrs);
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ObjectDrawerAttribute : Attribute
    {
        public Type type { get; private set; }
        public ObjectDrawerAttribute(Type type)
        {
            this.type = type;
        }
    }
}
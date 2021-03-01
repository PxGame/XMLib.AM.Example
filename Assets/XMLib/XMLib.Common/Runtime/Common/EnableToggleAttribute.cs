/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/5/17 13:46:01
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace XMLib
{
    /// <summary>
    /// EnableToggleAttribute
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EnableToggleAttribute : Attribute
    {
        public EnableToggleAttribute()
        {
        }

        public static Dictionary<string, bool> GetEnableDict(object target)
        {
            Type type = target.GetType();
            FieldInfo[] fields = type.GetFields();

            Dictionary<string, bool> enableDict = new Dictionary<string, bool>();
            foreach (var field in fields)
            {
                EnableToggleAttribute attr = field.GetCustomAttribute<EnableToggleAttribute>();
                if (attr == null)
                {
                    continue;
                }
                object obj = field.GetValue(target);
                if (obj == null)
                {
                    continue;
                }
                enableDict[field.Name] = obj.ConvertTo(false);
            }
            return enableDict;
        }
    }

    /// <summary>
    /// EnableToggleItemAttribute
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EnableToggleItemAttribute : Attribute
    {
        public string[] toggleNames { get; protected set; }
        public bool isTrue { get; protected set; }

        public EnableToggleItemAttribute(params string[] toggleNames)
            : this(true, toggleNames)
        {
        }

        public EnableToggleItemAttribute(bool isTrue, params string[] toggleNames)
        {
            this.toggleNames = toggleNames;
            this.isTrue = isTrue;
        }

        public static bool EnableChecker(FieldInfo field, Dictionary<string, bool> toggleDict)
        {
            EnableToggleItemAttribute attr = field.GetCustomAttribute<EnableToggleItemAttribute>();
            if (attr == null)
            {
                return true;
            }

            return attr.EnableChecker(toggleDict);
        }

        public bool EnableChecker(Dictionary<string, bool> toggleDict)
        {
            foreach (var toggleName in toggleNames)
            {
                if (!toggleDict.TryGetValue(toggleName, out bool enable))
                {
                    continue;
                }

                if ((isTrue && !enable) || (!isTrue && enable))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
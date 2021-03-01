/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/9/23 12:27:46
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// DataDictionary
    /// </summary>
    public class DataDictionary<K, V> : Dictionary<K, V> where K : struct
    {
        public bool TryGetValue(K key, Type type, out V data)
        {
            if (base.TryGetValue(key, out V value) && value != null && TypeUtility.ConvertToChecker(value.GetType(), type))
            {
                data = value;
                return true;
            }

            data = TypeUtility.GetDefaultValue<V>();
            return false;
        }

        public V GetOrCreateValue(K key, Type type)
        {
            if (TryGetValue(key, type, out V data))
            {
                return data;
            }

            if (!TypeUtility.ConvertToChecker(type, typeof(V)))
            {
                throw new RuntimeException($"{type} 无法转换到 {typeof(V)}");
            }

            this[key] = data = (V)TypeUtility.CreateInstance(type);
            return data;
        }


        #region extensions

        public bool TryGetValue<T>(K key, out T data) where T : V
        {
            if (TryGetValue(key, typeof(T), out V value))
            {
                data = (T)value;
                return true;
            }

            data = TypeUtility.GetDefaultValue<T>();
            return false;
        }

        public T GetOrCreateValue<T>(K key) where T : V
        {
            return (T)GetOrCreateValue(key, typeof(T));
        }

        public T GetValue<T>(K key, T defualtValue) where T : V
        {
            return TryGetValue<T>(key, out T data) ? data : defualtValue;
        }

        #endregion
    }
}
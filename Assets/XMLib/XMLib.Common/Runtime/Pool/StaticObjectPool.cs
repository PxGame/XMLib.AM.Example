/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/1/11 11:13:52
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace XMLib
{
    public static class ListPool<T>
    {
        private static readonly SimpleObjectPool<List<T>> _pool = new SimpleObjectPool<List<T>>(OnPush, null);

        private static void OnPush(List<T> obj)
        {
            obj.Clear();
        }

        public static List<T> Pop() => _pool.Pop();

        public static void Push(List<T> obj) => _pool.Push(obj);

        public static void Clear() => _pool.Clear();
    }

    public static class StaticPool<T> where T : class, new()
    {
        public static Action<T> onPush;
        public static Action<T> onPop;
        private static readonly SimpleObjectPool<T> _pool = new SimpleObjectPool<T>(onPush, onPop);

        public static T Pop() => _pool.Pop();

        public static void Push(T obj) => _pool.Push(obj);

        public static void Clear() => _pool.Clear();
    }
}
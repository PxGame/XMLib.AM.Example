/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/1/11 11:30:50
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// StaticObjectPool
    /// </summary>
    public class SimpleObjectPool<T> where T : class, new()
    {
        private Stack<T> _pool = new Stack<T>();
        private Action<T> _onPush;
        private Action<T> _onPop;

        public SimpleObjectPool(Action<T> onPush, Action<T> onPop)
        {
            _onPush = onPush;
            _onPop = onPop;
        }

        public void Push(T obj)
        {
            SuperLog.Assert(obj != null);
            _pool.Push(obj);
            _onPush?.Invoke(obj);
        }

        public T Pop()
        {
            T obj = _pool.Count > 0 ? _pool.Pop() : new T();
            _onPop?.Invoke(obj);
            return obj;
        }

        public void Clear()
        {
            _pool.Clear();
        }
    }
}
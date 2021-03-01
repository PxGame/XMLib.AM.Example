/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/9/15 19:45:32
 */

using System.Collections.Generic;

namespace XMLib
{
    /// <summary>
    /// ObjectPool
    /// </summary>
    public class ObjectPool<TTag>
    {
        private readonly Dictionary<TTag, Stack<object>> objStackDict;
        private readonly int primeSize;

        public ObjectPool(int prime = 32)
        {
            this.primeSize = prime;
            objStackDict = new Dictionary<TTag, Stack<object>>(prime);
        }

        public int Count(TTag tag)
        {
            return objStackDict.TryGetValue(tag, out Stack<object> pool) ? pool.Count : 0;
        }

        public void Push(IPoolItem<TTag> poolItem)
        {
            if (null == poolItem)
            {
                return;
            }

            Push(poolItem.poolTag, poolItem);

            poolItem.inPool = true;
            poolItem.OnPushPool();
        }

        public void Push(TTag tag, object item)
        {
            if (null == item)
            {
                return;
            }

            Stack<object> stack;
            if (!objStackDict.TryGetValue(tag, out stack))
            {
                stack = new Stack<object>(32);
                objStackDict.Add(tag, stack);
            }

            stack.Push(item);
        }

        public object Pop(TTag poolTag)
        {
            Stack<object> stack;
            if (!objStackDict.TryGetValue(poolTag, out stack) || 0 >= stack.Count)
            {
                return null;
            }

            object item = stack.Pop();

            if (item is IPoolItem<TTag> poolItem)
            {
                poolItem.inPool = false;
                poolItem.OnPopPool();
            }
            return item;
        }

        public T Pop<T>(TTag poolTag) where T : class
        {
            Stack<object> stack;

            if (!objStackDict.TryGetValue(poolTag, out stack) || 0 >= stack.Count)
            {
                return null;
            }

            T item = stack.Peek() as T;
            if (null == item)
            {
                return null;
            }

            stack.Pop();

            if (item is IPoolItem<TTag> poolItem)
            {
                poolItem.inPool = false;
                poolItem.OnPopPool();
            }

            return item;
        }

        public void Clear()
        {
            objStackDict.Clear();
        }
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/9/21 23:20:32
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace XMLib
{
    public interface IPackageData
    {
        object RawValue();
    }

    [Serializable]
    public struct P<T> : IPackageData
    {
        public T value;

        public object RawValue() => (T)this;

        public static implicit operator T(P<T> v) => v.value;

        public static implicit operator P<T>(T v) => new P<T>() { value = v };
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/9/28 15:03:53
 */

using System;
using UnityEngine;

namespace XMLib.Extensions
{
    /// <summary>
    /// LayerMask 扩展
    /// </summary>
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// 移除层级
        /// </summary>
        /// <param name="target"></param>
        /// <param name="removeLayer"></param>
        /// <returns></returns>
        public static LayerMask Remove(this LayerMask target, LayerMask removeLayer)
        {
            Int32 val1 = removeLayer.value;
            Int32 val2 = target.value;
            Int32 val3 = val2 & ~val1;
            LayerMask value = val3;
            return value;
        }

        /// <summary>
        /// 添加层级
        /// </summary>
        /// <param name="target"></param>
        /// <param name="addLayer"></param>
        /// <returns></returns>
        public static LayerMask Add(this LayerMask target, LayerMask addLayer)
        {
            Int32 val1 = addLayer.value;
            Int32 val2 = target.value;
            Int32 val3 = val2 | val1;
            LayerMask value = val3;
            return value;
        }

        /// <summary>
        /// 存在层级
        /// </summary>
        /// <param name="target"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool Exist(this LayerMask target, int layer)
        {
            int val1 = 1 << layer;
            int val2 = target.value;
            if ((val1 & val2) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
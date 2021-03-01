/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/11/4 12:26:48
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib.Extensions
{
    /// <summary>
    /// VectorExtensions
    /// </summary>
    public static class VectorExtensions
    {
        public static Vector2 Round(this in Vector2 vector, int decimals)
        {
            return new Vector2(
                vector.x.Round(decimals),
                vector.y.Round(decimals));
        }

        public static Vector3 Round(this in Vector3 vector, int decimals)
        {
            return new Vector3(
                vector.x.Round(decimals),
                vector.y.Round(decimals),
                vector.z.Round(decimals));
        }
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/2/20 16:27:24
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib.Extensions
{
    /// <summary>
    /// GameObjectExtensions
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 获取物体在场景中的位置
        /// </summary>
        /// <param name="GameObject"></param>
        /// <returns></returns>
        public static string GetScenePath(this GameObject go)
        {
            if (go != null && go.transform != null)
            {
                return go.transform.GetScenePath();
            }
            return string.Empty;
        }
    }
}
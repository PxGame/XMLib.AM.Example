/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/9/28 15:03:53
 */

using UnityEngine;

namespace XMLib.Extensions
{
    /// <summary>
    /// RectTransform扩展
    /// </summary>
    public static class RectTransformExtensions
    {
        /// <summary>
        /// 获取中心偏移
        /// </summary>
        /// <returns>中心偏移像素</returns>
        public static Vector2 GetCenterOffset(this RectTransform trans)
        {
            return Vector2.Scale(trans.rect.size, Vector2.one * 0.5f - trans.pivot);
        }

        /// <summary>
        /// 中心坐标
        /// </summary>
        /// <returns>屏幕坐标</returns>
        public static Vector2 GetCenterPosition(this RectTransform trans)
        {
            return (Vector2)trans.position + trans.GetCenterOffset();
        }

        /// <summary>
        /// 中心坐标
        /// </summary>
        /// <returns>屏幕坐标</returns>
        public static Vector2 GetCenterLocalPosition(this RectTransform trans)
        {
            return (Vector2)trans.localPosition + trans.GetCenterOffset();
        }

        /// <summary>
        /// 设置中心坐标
        /// </summary>
        /// <param name="position">屏幕坐标</param>
        public static void SetCenterPosition(this RectTransform trans, Vector2 position)
        {
            trans.position = position + trans.GetCenterOffset();
        }

        /// <summary>
        /// 设置中心坐标
        /// </summary>
        /// <param name="position">屏幕坐标</param>
        public static void SetCenterLocalPosition(this RectTransform trans, Vector2 position)
        {
            trans.localPosition = position + trans.GetCenterOffset();
        }
    }
}
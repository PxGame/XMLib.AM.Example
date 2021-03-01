/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/9/28 15:03:53
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib.Extensions
{
    /// <summary>
    /// FloatExtension
    /// </summary>
    public static class FloatExtensions
    {
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax) => MathUtility.Remap(value, fromMin, fromMax, toMin, toMax);

        public static float Remap01(this float value, float fromMin, float fromMax) => MathUtility.Remap01(value, fromMin, fromMax);

        public static float Round(this float value, int decimals) => (float)Math.Round(value, decimals);

        public static bool IsEqualToZero(this float value) => Mathf.Abs(value) < Mathf.Epsilon;

        public static bool NotEqualToZero(this float value) => Mathf.Abs(value) > Mathf.Epsilon;

        /// <summary>
        /// Wraps a float between -180 and 180.
        /// </summary>
        /// <param name="value">The float to wrap.</param>
        /// <returns>A value between -180 and 180.</returns>
        public static float Wrap180(this float value)
        {
            value %= 360.0f;
            if (value < -180.0f)
            {
                value += 360.0f;
            }
            else if (value > 180.0f)
            {
                value -= 360.0f;
            }
            return value;
        }

        /// <summary>
        /// Wraps a float between 0 and 1.
        /// </summary>
        /// <param name="value">The float to wrap.</param>
        /// <returns>A value between 0 and 1.</returns>
        public static float Wrap1(this float value)
        {
            value %= 1.0f;
            if (value < 0.0f)
            {
                value += 1.0f;
            }
            return value;
        }

        /// <summary>
        /// Gets the fraction portion of a float.
        /// </summary>
        /// <param name="value">The float.</param>
        /// <returns>The fraction portion of a float.</returns>
        public static float GetFraction(this float value) => value - Mathf.Floor(value);
    }
}
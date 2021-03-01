/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/1/24 23:59:01
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// ByteAngle
    /// </summary>
    public static class ByteAngle
    {
        public const byte ByteAngleScale = 2;

        /// <summary>
        /// byte 映射到角度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ByteToAngle(byte value)
        {
            return value * ByteAngleScale;
        }

        public static byte AngleToByte(float angle)
        {
            angle = angle % 360f;
            angle += angle >= 0 ? 0 : 360f;
            return (byte)(angle == 0 ? 0 : (angle / ByteAngleScale));
        }

        public static float FixedByteAngle(float angle)
        {
            return ByteToAngle(AngleToByte(angle));
        }

        public static float AngleYFromDir(Vector3 dir)
        {
            dir.y = 0f;//在同一平面旋转
            return Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
        }

        public static Vector3 AngleYToDir(float yAngle)
        {
            return (Quaternion.AngleAxis(yAngle, Vector3.up) * Vector3.forward).normalized;
        }

        public static byte ByteAngleYFromDir(Vector3 dir)
        {
            return AngleToByte(AngleYFromDir(dir));
        }

        public static float FixedByteAngleYFromDir(Vector3 dir)
        {
            return FixedByteAngle(AngleYFromDir(dir));
        }
    }
}
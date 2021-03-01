/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/25 9:59:35
 */

using System;

using UnityEngine;
using UnityEngine.Assertions;

namespace XMLib
{
    /// <summary>
    /// 数学工具
    /// </summary>
    public static class MathUtility
    {
        /// <summary>
        /// 浮点数精度
        /// </summary>
        public const float FEpsilon = 0.000001f;

        public const float PI = Mathf.PI;
        public const float PI2 = PI * 2.0f;

        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float scale = Mathf.Clamp01((value - fromMin) / (fromMax - fromMin));
            value = (toMax - toMin) * scale + toMin;
            return value;
        }

        public static int Remap(int value, int fromMin, int fromMax, int toMin, int toMax)
        {
            float scale = Mathf.Clamp01((value - fromMin) / (float)(fromMax - fromMin));
            value = Mathf.RoundToInt((toMax - toMin) * scale) + toMin;
            return value;
        }

        public static float Remap01(float value, float fromMin, float fromMax)
        {
            return Remap(value, fromMin, fromMax, 0, 1);
        }

        public static int Remap01(int value, int fromMin, int fromMax)
        {
            return Remap(value, fromMin, fromMax, 0, 1);
        }

        /// <summary>
        /// 跳跃速度
        /// </summary>
        /// <param name="gravity">重力</param>
        /// <param name="height">高度</param>
        /// <returns>速度</returns>
        public static float JumpSpeed(float gravity, float height)
        {
            return Mathf.Sqrt(2.0f * Mathf.Abs(gravity) * height);
        }

        /// <summary>
        /// 浮点数是否等于0
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否等于0</returns>
        public static bool FEqualZero(float value)
        {
            return Mathf.Abs(value) < FEpsilon;
        }

        /// <summary>
        /// 浮点数是否等于0
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否等于0</returns>
        public static bool FEqualZero(Vector2 value)
        {
            return FEqualZero(value.x) && FEqualZero(value.y);
        }

        /// <summary>
        /// 浮点数是否等于0
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否等于0</returns>
        public static bool FEqualZero(Vector3 value)
        {
            return FEqualZero(value.x) && FEqualZero(value.y) && FEqualZero(value.z);
        }

        /// <summary>
        /// 小于精度时则置0
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>结果</returns>
        public static float FFixed(float value)
        {
            return Mathf.Abs(value) < FEpsilon ? 0f : value;
        }

        /// <summary>
        /// 小于精度时则置0
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>结果</returns>
        public static Vector2 FFixedV2(Vector2 value)
        {
            value.x = FFixed(value.x);
            value.y = FFixed(value.y);
            return value;
        }

        /// <summary>
        /// 小于精度时则置0
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>结果</returns>
        public static Vector3 FFixedV3(Vector3 value)
        {
            value.x = FFixed(value.x);
            value.y = FFixed(value.y);
            value.z = FFixed(value.z);
            return value;
        }

        /// <summary>
        /// 小于精度时则置0
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>结果</returns>
        public static Vector3 FFixedV4(Vector4 value)
        {
            value.x = FFixed(value.x);
            value.y = FFixed(value.y);
            value.z = FFixed(value.z);
            value.w = FFixed(value.w);
            return value;
        }

        /// <summary>
        /// 浮点数是否相等
        /// </summary>
        /// <param name="left">值1</param>
        /// <param name="right">值2</param>
        /// <returns>是否相等</returns>
        public static bool FEqual(float left, float right)
        {
            return Mathf.Abs(left - right) < FEpsilon;
        }


        /// <summary>
        /// CalcBoxVertex 计算得到的顶点，绘制序列，4个为一个面，共6个面，顺序为上下左右前后
        /// </summary>
        /// <returns></returns>
        public static int[] GetBoxSurfaceIndex()
        {
            return new int[] {
                0,1,2,3,//上
                4,5,6,7,//下
                2,6,5,3,//左
                0,4,7,1,//右
                1,7,6,2,//前
                0,3,5,4//后
            };
        }

        /// <summary>
        /// 计算box 的 8个顶点
        /// <para>顺时针顶面 [0,1,2,3]</para>
        /// <para>顺时针底面 [4,5,6,7]</para>
        /// </summary>
        public static Vector3[] CalcBoxVertex(Vector3 size)
        {
            Vector3 halfSize = size / 2f;

            Vector3[] points = new Vector3[8];

            //上面-顺时针
            //  3 → 0
            //  ↑      ↓
            //  2 ← 1
            points[0] = new Vector3(halfSize.x, halfSize.y, halfSize.z);
            points[1] = new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            points[2] = new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            points[3] = new Vector3(-halfSize.x, halfSize.y, halfSize.z);

            //下面-顺时针
            //  5 ← 4
            //  ↓      ↑
            //  6 → 7
            points[4] = new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            points[5] = new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            points[6] = new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            points[7] = new Vector3(halfSize.x, -halfSize.y, -halfSize.z);

            return points;
        }

        /// <summary>
        /// 计算box 的 8个顶点
        /// </summary>
        /// <param name="size"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3[] CalcBoxVertex(Vector3 size, Matrix4x4 matrix)
        {
            Vector3[] points = CalcBoxVertex(size);

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = matrix.MultiplyPoint(points[i]);
            }

            return points;
        }

        /// <summary>
        /// 计算方形顶点序列
        /// </summary>
        public static Vector3[] CalcRectVertex(Vector2 size, Matrix4x4 matrix)
        {
            float halfW = size.x / 2;
            float halfH = size.y / 2;

            Vector3[] vertices = new Vector3[4]
            {
                new Vector3 (-halfW, halfH),
                new Vector3 (halfW, halfH),
                new Vector3 (halfW, -halfH),
                new Vector3 (-halfW, -halfH),
            };

            vertices[0] = matrix.MultiplyPoint(vertices[0]);
            vertices[1] = matrix.MultiplyPoint(vertices[1]);
            vertices[2] = matrix.MultiplyPoint(vertices[2]);
            vertices[3] = matrix.MultiplyPoint(vertices[3]);

            return vertices;
        }

        /// <summary>
        /// 计算圆形顶点序列
        /// </summary>
        public static Vector3[] CalcCircleVertex(float radius, Matrix4x4 matrix, int subdivide = 30)
        {
            float deg = 2 * Mathf.PI;
            float deltaDeg = deg / subdivide;
            Vector3[] vertices = new Vector3[subdivide];
            for (int i = 0; i < subdivide; i++)
            {
                Vector2 pos;
                float d = deg - deltaDeg * i;
                pos.x = radius * Mathf.Cos(d);
                pos.y = radius * Mathf.Sin(d);

                vertices[i] = matrix.MultiplyPoint(pos);
            }

            return vertices;
        }

        /// <summary>
        /// 计算扇形顶点序列，旋转以Vector3.right为起点，Vector3.forward为旋转轴
        /// </summary>
        public static Vector3[] CalcArcVertex(float radius, float angle, float rotation, Matrix4x4 matrix, bool hasCenter = true, int subdivide = 30)
        {
            if (angle <= 0)
            {
                return Array.Empty<Vector3>();
            }

            angle = angle % 360f;
            rotation = rotation % 360f;

            float degree = angle * Mathf.Deg2Rad;
            float startDegree = rotation * Mathf.Deg2Rad - degree / 2.0f;

            int divide = (int)(degree / (Mathf.PI * 2) * subdivide);
            if (divide <= 0)
            {
                divide = 3;
            }

            float deltaDeg = degree / divide;

            Vector3[] vertices = new Vector3[divide + 1 + (hasCenter ? 1 : 0)];
            for (int i = 0; i <= divide; i++)
            {
                Vector2 pos;
                float d = startDegree + deltaDeg * i;
                pos.x = radius * Mathf.Cos(d);
                pos.y = radius * Mathf.Sin(d);

                vertices[i] = matrix.MultiplyPoint(pos);
            }
            if (hasCenter)
            {
                vertices[vertices.Length - 1] = matrix.MultiplyPoint(Vector3.zero);
            }

            return vertices;
        }

        /// <summary>
        /// 计算胶囊顶点序列
        /// </summary>
        public static Vector3[] CalcCapsuleVertex2D(float height, float radius, Matrix4x4 matrix, int subdivide = 30)
        {
            float diameter = 2.0f * radius;
            height = height < diameter ? diameter : height;
            float halfHeight = (height - diameter) / 2.0f;
            Vector3[] upArc = CalcArcVertex(radius, 180, 90, matrix * Matrix4x4.Translate(Vector3.up * halfHeight), false, subdivide);
            Vector3[] downArc = CalcArcVertex(radius, 180, -90, matrix * Matrix4x4.Translate(Vector3.down * halfHeight), false, subdivide);
            return ArrayUtility.Combine(upArc, downArc);
        }

        /// <summary>
        /// 矩阵合并
        /// 合并矩阵，用于简化计算
        /// 合并顺序为反向矩阵序列相乘
        /// </summary>
        /// <param name="matrices">由内向外矩阵序列，最近的矩阵在最前面</param>
        /// <returns>计算结果</returns>
        public static Matrix4x4 CombineMatrix4x4(params Matrix4x4[] matrices)
        {
            if (matrices == null || matrices.Length == 0)
            {
                return Matrix4x4.identity;
            }

            int cnt = matrices.Length;
            Matrix4x4 combine = matrices[cnt - 1];
            //合并矩阵，矩阵序列反向相乘
            for (int j = cnt - 2; j >= 0; j--)
            {
                combine *= matrices[j];
            }

            return combine;
        }
    }
}
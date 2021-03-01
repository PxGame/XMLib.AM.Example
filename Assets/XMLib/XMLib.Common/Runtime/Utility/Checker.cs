/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/5/25 11:05:58
 */

using System.Diagnostics;

namespace XMLib
{
    /// <summary>
    /// Checker
    /// </summary>
    public static class Checker
    {
        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public static void IsTrue(bool value, string msg)
        {
            if (value)
            {
                return;
            }

            throw new RuntimeException(msg);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public static void NotNull(object obj, string msg = "")
        {
            IsTrue(obj != null, msg);
        }
    }
}
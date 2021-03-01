/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/11/4 17:30:13
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// 日志工具
    /// </summary>
    public class SuperLog
    {
        public static string tag { get; set; } = "XMLib";

        public static bool enable { get; set; } = true;

        public static void Log(LogType type, string msg)
        {
            if (!enable)
            {
                return;
            }

            UnityEngine.Debug.unityLogger.Log(type, tag, msg);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                Log(LogType.Assert, message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition)
        {
            Assert(condition, "Assertion failed");
        }

        public static void LogException(Exception exception)
        {
            UnityEngine.Debug.unityLogger.LogException(exception);
        }

        public static void Log(string msg)
        {
            Log(LogType.Log, msg);
        }

        public static void LogError(string msg)
        {
            Log(LogType.Error, msg);
        }

        public static void LogWarning(string msg)
        {
            Log(LogType.Warning, msg);
        }
    }

    /// <summary>
    /// 日志工具
    /// </summary>
    public class SuperLogHandler
    {
        private readonly Func<string> title;
        private readonly string titleColor;
        private readonly SuperLogHandler parent;
        private bool _enable = true;

        public bool enable
        {
            get => _enable && (parent == null || parent.enable);
            set => _enable = value;
        }

        public static SuperLogHandler Create(Func<string> titleBuilder, Color? titleColor = null, SuperLogHandler parent = null)
        {
            return new SuperLogHandler(titleBuilder, titleColor, parent);
        }

        public static SuperLogHandler Create(string title, Color? titleColor = null, SuperLogHandler parent = null)
        {
            return new SuperLogHandler(() => title, titleColor, parent);
        }

        private SuperLogHandler(Func<string> title, Color? titleColor = null, SuperLogHandler parent = null)
        {
            if (!titleColor.HasValue)
            {
                UnityEngine.Random.InitState(title().GetHashCode());
                Color color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
                titleColor = color;
            }

            this.title = title;
            this.titleColor = ColorUtility.ToHtmlStringRGB(titleColor.Value);
            this.parent = parent;

            //检查是否循环依赖
            while (parent != null)
            {
                if (parent == this)
                {//如果发生循环依赖，则移除父节点
                    this.parent = null;
                    SuperLog.LogError("SuperLogHandler 发现循环依赖");
                    break;
                }
                parent = parent.parent;
            }
        }

        public SuperLogHandler CreateSub(string title, Color? titleColor = null)
        {
            return Create(title, titleColor, this);
        }

        public SuperLogHandler CreateSub(Func<string> titleBuilder, Color? titleColor = null)
        {
            return Create(titleBuilder, titleColor, this);
        }

        protected string GetTitleStr()
        {
            string titleStr = null;
            try
            {
                StringBuilder builder = new StringBuilder();
                if (parent != null)
                {
                    builder.Append(parent.GetTitleStr());
                }
                if (title != null)
                {
                    builder.Append($"[<color=#{titleColor}>{title()}</color>]");
                }
                titleStr = builder.ToString();
            }
            catch (Exception ex)
            {
                titleStr = "<color=red>[Title Exception]</color>";
                SuperLog.LogWarning($"SuperLogHandler.GetTitleStr Exception !!!\n{ex}");
            }
            return titleStr;
        }

        private string CreateMsg(string msg, params object[] args)
        {
            string titleStr = GetTitleStr();
            return string.Format($"{titleStr}{msg}", args);
        }

        public void Log(string msg, params object[] args)
        {
            if (!enable)
            {
                return;
            }
            SuperLog.Log(CreateMsg(msg, args));
        }

        public void LogError(string msg, params object[] args)
        {
            if (!enable)
            {
                return;
            }
            SuperLog.LogError(CreateMsg(msg, args));
        }

        public void LogWarning(string msg, params object[] args)
        {
            if (!enable)
            {
                return;
            }
            SuperLog.LogWarning(CreateMsg(msg, args));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public void Assert(bool condition, string msg, params object[] args)
        {
            if (!enable)
            {
                return;
            }
            SuperLog.Assert(condition, CreateMsg(msg, args));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public void Assert(bool condition)
        {
            if (!enable)
            {
                return;
            }
            SuperLog.Assert(condition, CreateMsg("Assertion failed"));
        }
    }
}
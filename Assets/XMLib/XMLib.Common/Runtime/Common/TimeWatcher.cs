/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 12/20/2018 9:41:15 AM
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace XMLib
{
    /// <summary>
    /// 时间察看者
    /// </summary>
    public class TimeWatcher : IDisposable
    {
        /// <summary>
        /// 检查点
        /// </summary>
        private struct CheckPoint
        {
            public string Tag { get; private set; }
            public long Time { get; private set; }
            public long Interval { get; private set; }

            public CheckPoint(string tag, long time, long interval)
            {
                Tag = tag;
                Time = time;
                Interval = interval;
            }

            public override string ToString()
            {
                return string.Format("{0} 时间 : {1} ms, 间隔 : {2} ms", Tag, Time, Interval);
            }
        }

        private string _watcherTag;
        private List<CheckPoint> _points;
        private Stopwatch _stopwatch;
        private long _lastTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tag">标签</param>
        public TimeWatcher(string tag)
        {
            _watcherTag = tag;
            _points = new List<CheckPoint>();
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _lastTime = 0;
        }

        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="tag">记录名</param>
        public void End(string tag)
        {
            long currentTime = _stopwatch.ElapsedMilliseconds;
            long interval = currentTime - _lastTime;

            _points.Add(new CheckPoint(tag, currentTime, interval));

            _lastTime = currentTime;
        }

        /// <summary>
        /// 更新最后检查时间为当前时间，用于精准的计算下一次End的时间间隔
        /// </summary>
        public void Start()
        {
            _lastTime = _stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        private void Show()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("[{0}]", _watcherTag);

            if (_points.Count > 0)
            {
                builder.Append("\n检查点:\n");
                foreach (CheckPoint point in _points)
                {
                    builder.AppendFormat("{0}\n", point);
                }
            }

            builder.AppendFormat("总时间:{0} ms", _stopwatch.ElapsedMilliseconds);

            //

            SuperLog.Log(builder.ToString());

            //
        }

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                _stopwatch.Stop();

                Show();

                _stopwatch = null;
                disposedValue = true;
            }
        }

        ~TimeWatcher()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
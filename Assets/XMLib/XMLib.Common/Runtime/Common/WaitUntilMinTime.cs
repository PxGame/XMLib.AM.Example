/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/9/16 7:01:08
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLib;

namespace AliveCell
{
    /// <summary>
    /// WaitUntilMinTime
    /// </summary>
    public sealed class WaitUntilMinTime : CustomYieldInstruction
    {
        private Func<bool> predicate;
        private float waitMinTime;
        private bool useTimeScale;
        private float startTime;

        private float currentTime => useTimeScale ? Time.time : Time.unscaledTime;
        private bool timeOver => currentTime - startTime >= waitMinTime;
        public override bool keepWaiting => !(timeOver && predicate());

        public WaitUntilMinTime(float waitMinTime, bool useTimeScale, Func<bool> predicate)
        {
            this.waitMinTime = waitMinTime;
            this.useTimeScale = useTimeScale;
            this.predicate = predicate;
            this.startTime = currentTime;
        }

    }
}
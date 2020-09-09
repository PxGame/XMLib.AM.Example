/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 2019/12/26 0:27:41
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using XMLib;
using XMLib.AM;

[Flags]
public enum CheckType : byte
{
    None = 0b0000,
    FowardWall = 0b0001,
    Ground = 0b0010,
    Air = 0b0100,

    /// <summary>
    /// 部分匹配模式
    /// </summary>
    KeyCode = 0b1000,

    /// <summary>
    /// 全部匹配模式
    /// </summary>
    KeyCodeAll = 0b0001_0000,
}

/// <summary>
/// TSConditionConfig
/// </summary>
[ActionConfig(typeof(Condition))]
public class ConditionConfig : HoldFrames
{
    #region Items

    public interface IItem
    {
        bool Execute(IActionMachine machine, IActionController controller);
    }

    public class GroundChecker : IItem
    {
        public bool isNot;

        public bool Execute(IActionMachine machine, IActionController controller)
        {
            bool result = controller.CheckGround();
            return isNot ? !result : result;
        }
    }

    public class AirChecker : IItem
    {
        public bool isNot;

        public bool Execute(IActionMachine machine, IActionController controller)
        {
            bool result = controller.CheckAir();
            return isNot ? !result : result;
        }
    }

    public class KeyCodeChecker : IItem
    {
        public ActionKeyCode keyCode;
        public bool isNot;
        public bool fullMatch;

        public bool Execute(IActionMachine machine, IActionController controller)
        {
            bool result = machine.input.HasKey(controller.ID, (int)keyCode, fullMatch);
            return isNot ? !result : result;
        }
    }

    public class VeclocityChecker : IItem
    {
        public float horizontalVelocity;
        public float verticalVelocity;
        public CompareType horzontalCmp;
        public CompareType verticalCmp;

        public bool Execute(IActionMachine machine, IActionController controller)
        {
            if (horzontalCmp != CompareType.none && !CompareUtility.Compare(new Vector2(controller.velocity.x, controller.velocity.z).magnitude, horizontalVelocity, horzontalCmp))
            {
                return false;
            }
            if (verticalCmp != CompareType.none && !CompareUtility.Compare(controller.velocity.y, verticalVelocity, verticalCmp))
            {
                return false;
            }

            return true;
        }
    }

    #endregion Items

    public string stateName;
    public int priority;

    /// <summary>
    /// 延迟调用跳转，动作最后一帧执行跳转,必须启用EnableBeginEnd，否则无效
    /// </summary>
    [EnableToggle()]
    [EnableToggleItem(nameof(enableBeginEnd))]
    public bool delayInvoke;

    /// <summary>
    /// 每一帧都需要为真，才能在最后跳转
    /// </summary>
    [EnableToggleItem(nameof(delayInvoke), nameof(enableBeginEnd))]
    public bool allFrameCheck;

    [ConditionTypes]
    public List<IItem> checker;

    public override string ToString()
    {
        return $"{GetType().Name} > {stateName} - {priority}";
    }
}

/// <summary>
/// TSCondition
/// </summary>
public class Condition : IActionHandler
{
    public void Enter(ActionNode node)
    {
        ConditionConfig config = (ConditionConfig)node.config;
        //IActionMachine machine = node.actionMachine;
        //IActionController controller = (IActionController)node.actionMachine.controller;
        node.data = 0;

        //校验
        if (config.delayInvoke && !config.EnableBeginEnd())
        {
            throw new RuntimeException($"使用延迟调用(DelayInvoke)，必须启用区间(EnableBeginEnd)\n{node}");
        }
        //
    }

    public void Exit(ActionNode node)
    {
        //TSConditionConfig config = (TSConditionConfig)node.config;
        //IActionMachine machine = node.actionMachine;
        //IActionController controller = (IActionController)node.actionMachine.controller;
    }

    public void Update(ActionNode node, float deltaTime)
    {
        ConditionConfig config = (ConditionConfig)node.config;
        IActionMachine machine = node.actionMachine;
        IActionController controller = (IActionController)node.actionMachine.controller;

        if (!config.delayInvoke || !config.EnableBeginEnd())
        {
            if (!Checker(config.checker, machine, controller))
            {
                return;
            }
        }
        else
        {
            int successCnt = (int)node.data;
            if (Checker(config.checker, machine, controller))
            {//为true时计数+1
                node.data = ++successCnt;
            }

            if (successCnt == 0//true的次数为0
            || config.allFrameCheck && successCnt != (node.updateCnt + 1)//每一帧都必须为true,updateCnt需要+1是因为updateCnt在Update后才会递增
            || machine.GetStateFrameIndex() != config.GetEndFrame())//不是最后一帧
            {
                return;
            }
        }

        machine.ChangeState(config.stateName, config.priority);
    }

    public static bool Checker(List<ConditionConfig.IItem> checkers, IActionMachine machine, IActionController controller)
    {
        if (checkers == null || checkers.Count == 0)
        {
            return true;
        }

        foreach (var checker in checkers)
        {
            if (!checker.Execute(machine, controller))
            {
                return false;
            }
        }

        return true;
    }
}
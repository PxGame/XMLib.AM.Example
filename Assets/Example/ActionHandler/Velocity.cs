/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 2019/11/22 12:02:44
 */

using UnityEngine;
using XMLib;
using XMLib.AM;

/// <summary>
/// VelocityConfig
/// </summary>
[ActionConfig(typeof(Velocity))]
[System.Serializable]
public class VelocityConfig : HoldFrames
{
    public Vector3 velocity;

    [EnableToggle()]
    public bool fixDir;


    [EnableToggleItem(nameof(fixDir))]
    public bool useInput;

    public bool useHeight;
}

/// <summary>
/// SVelocity
/// </summary>
public class Velocity : IActionHandler
{
    public void Enter(ActionNode node)
    {
        VelocityConfig config = (VelocityConfig)node.config;
        IActionMachine machine = node.actionMachine;
        IActionController controller = (IActionController)node.actionMachine.controller;

        Vector3 velocity;
        if (config.fixDir)
        {
            if (config.useInput)
            {
                var inputData = machine.input.GetRawInput(controller.ID);
                velocity = inputData.GetRotation() * config.velocity;
            }
            else
            {
                velocity = controller.rotation * config.velocity;
            }
        }
        else
        {
            velocity = config.velocity;
        }

        if (config.useHeight)
        {
            velocity.y = MathUtility.JumpSpeed(controller.gravity, config.velocity.y);
        }

        controller.velocity = velocity;
    }

    public void Exit(ActionNode node)
    {
        //SApplyForceConfig config = (SApplyForceConfig)node.config;
        //IActionMachine machine = node.actionMachine;
        //IActionController controller = (IActionController)node.actionMachine.controller;
    }

    public void Update(ActionNode node, float deltaTime)
    {
        //SApplyForceConfig config = (SApplyForceConfig)node.config;
        //IActionMachine machine = node.actionMachine;
        //IActionController controller = (IActionController)node.actionMachine.controller;
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 2019/11/24 0:02:52
 */

using UnityEngine;
using XMLib.AM;

/// <summary>
/// SMoveConfig
/// </summary>
[ActionConfig(typeof(Move))]
[System.Serializable]
public class MoveConfig
{
    public float moveSpeed;
}

/// <summary>
/// SMove
/// </summary>
public class Move : IActionHandler
{
    public void Enter(ActionNode node)
    {
        MoveConfig config = (MoveConfig)node.config;
        IActionMachine machine = node.actionMachine;
        //IActionController controller = (IActionController)node.actionMachine.controller;
    }

    public void Exit(ActionNode node)
    {
        MoveConfig config = (MoveConfig)node.config;
        IActionMachine machine = node.actionMachine;
        //IActionController controller = (IActionController)node.actionMachine.controller;
    }

    public void Update(ActionNode node, float deltaTime)
    {
        MoveConfig config = (MoveConfig)node.config;
        IActionMachine machine = node.actionMachine;
        IActionController controller = (IActionController)node.actionMachine.controller;

        Vector3 velocity = controller.velocity;

        if (machine.input.HasKey(controller.ID, (int)ActionKeyCode.Axis))
        {
            InputData data = machine.input.GetRawInput(controller.ID);

            velocity = data.GetDir() * config.moveSpeed;
            controller.rotation = data.GetRotation();
        }

        controller.velocity = new Vector3(velocity.x, controller.velocity.y, velocity.z);
    }
}
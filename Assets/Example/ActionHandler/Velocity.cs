using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLib.AM;

[System.Serializable]
[ActionConfig(typeof(Velocity))]
public class VelocityConfig : HoldFrames
{
    public Vector3 velocity;
}

public class Velocity : IActionHandler
{
    public void Enter(ActionNode node)
    {
        VelocityConfig config = (VelocityConfig)node.config;
        IActionMachine machine = node.actionMachine;
        ActionMachineController controller = (ActionMachineController)node.actionMachine.controller;

        controller.rigid.velocity = controller.modelRotation * config.velocity;
    }

    public void Exit(ActionNode node)
    {
    }

    public void Update(ActionNode node, float deltaTime)
    {
    }
}
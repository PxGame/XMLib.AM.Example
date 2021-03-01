using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLib.AM;

[System.Serializable]
[ActionConfig(typeof(Move))]
public class MoveConfig
{
    public float moveSpeed;
}

public class Move : IActionHandler
{
    public void Enter(ActionNode node)
    {
    }

    public void Exit(ActionNode node)
    {
    }

    public void Update(ActionNode node, float deltaTime)
    {
        MoveConfig config = (MoveConfig)node.config;
        IActionMachine machine = node.actionMachine;
        ActionMachineController controller = (ActionMachineController)node.actionMachine.controller;

        if (InputData.HasEvent(InputEvents.Moving))
        {
            var velocity = controller.rigid.velocity;
            var move = InputData.axisValue.normalized * config.moveSpeed;

            velocity.x = move.x;
            velocity.z = move.y;

            controller.rigid.velocity = velocity;
        }
    }
}
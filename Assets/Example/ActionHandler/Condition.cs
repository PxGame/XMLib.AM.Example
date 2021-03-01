using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLib;
using XMLib.AM;

[System.Serializable]
[ActionConfig(typeof(Condition))]
public class ConditionConfig : HoldFrames
{
    public string stateName;
    public int priority;

    [SerializeReference]
    [Conditions.ConditionTypes]
    public List<Conditions.IItem> checker;

    public override string ToString()
    {
        return $"{GetType().Name} > {stateName} - {priority}";
    }
}

public class Condition : IActionHandler
{
    public void Enter(ActionNode node)
    {
    }

    public void Exit(ActionNode node)
    {
    }

    public void Update(ActionNode node, float deltaTime)
    {
        ConditionConfig config = (ConditionConfig)node.config;
        IActionMachine machine = node.actionMachine;
        ActionMachineController controller = (ActionMachineController)node.actionMachine.controller;

        if (Checker(config.checker, node))
        {
            machine.ChangeState(config.stateName, config.priority);
        }
    }

    public static bool Checker(List<Conditions.IItem> checkers, ActionNode node)
    {
        if (checkers == null || checkers.Count == 0)
        {
            return true;
        }

        foreach (var checker in checkers)
        {
            if (!checker.Execute(node))
            {
                return false;
            }
        }

        return true;
    }
}

namespace Conditions
{
    public class ConditionTypesAttribute : ObjectTypesAttribute
    {
        public override Type baseType => typeof(IItem);
    }

    public interface IItem
    {
        bool Execute(ActionNode node);
    }

    [Serializable]
    public class KeyCodeChecker : IItem
    {
        public InputEvents events;
        public bool isNot;
        public bool fullMatch;

        public bool Execute(ActionNode node)
        {
            IActionMachine machine = node.actionMachine;
            ActionMachineController controller = (ActionMachineController)node.actionMachine.controller;
            bool result = InputData.HasEvent(events, fullMatch);
            return isNot ? !result : result;
        }
    }
}
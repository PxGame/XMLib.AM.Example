using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XMLib;
using XMLib.AM;

public enum InputEvents
{
    None,
    Moving,
    Attack
}

public static class InputData
{
    public static InputEvents inputEvents { get; set; } = InputEvents.None;
    public static Vector2 axisValue { get; set; } = Vector2.zero;

    public static bool HasEvent(InputEvents e, bool fullMatch = false)
    {
        return fullMatch ? ((inputEvents & e) == inputEvents) : ((inputEvents & e) != 0);
    }

    public static void Clear()
    {
        inputEvents = InputEvents.None;
        axisValue = Vector2.zero;
    }
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    protected List<ActionMachineController> controllers;

    [SerializeField]
    protected List<TextAsset> configs;

    protected float logicTimer = 0f;
    protected const float logicDeltaTime = 1 / 30f;

    #region Input

    public GameInput input;

    #endregion Input

    private void Awake()
    {
        //初始化配置文件加载函数
        ActionMachineHelper.Init(OnActionMachineConfigLoader);
        input = new GameInput();
        input.Enable();

        Physics.autoSimulation = false;
    }

    private MachineConfig OnActionMachineConfigLoader(string configName)
    {
        TextAsset asset = configs.Find(t => string.Compare(t.name, configName) == 0);
        return DataUtility.FromJson<MachineConfig>(asset.text);
    }

    private void OnDestroy()
    {
        input.Disable();
    }

    private void Update()
    {
        UpdateInput();
        LogicUpdate();
    }

    private void UpdateInput()
    {
        var player = input.Player;
        var move = player.Move.ReadValue<Vector2>();
        if (player.Move.phase == InputActionPhase.Started)
        {
            InputData.inputEvents |= InputEvents.Moving;
            InputData.axisValue = move;
        }

        if (player.Attack.triggered)
        {
            InputData.inputEvents |= InputEvents.Attack;
        }
    }

    private void LogicUpdate()
    {
        logicTimer += Time.deltaTime;
        if (logicTimer >= logicDeltaTime)
        {
            logicTimer -= logicDeltaTime;

            RunLogicUpdate(logicDeltaTime);
        }
    }

    private void RunLogicUpdate(float logicDeltaTime)
    {
        foreach (var item in controllers)
        {
            item.LogicUpdate(logicDeltaTime);
        }

        //更新物理
        Physics.Simulate(logicDeltaTime);

        //清理输入
        InputData.Clear();
    }
}
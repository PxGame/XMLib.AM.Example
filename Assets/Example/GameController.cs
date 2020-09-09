using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XMLib;
using XMLib.AM;

public class GameController : MonoBehaviour, IInputContainer
{
    public int logicFramePerSecond = 30;
    [Range(0f, 2.0f)]
    public float logicTimeScale = 1f;
    public int inputTargetId;
    public List<TextAsset> configAssets;
    public List<PlayerController> players;

    protected Dictionary<string, MachineConfig> configDict = new Dictionary<string, MachineConfig>();
    protected float perFrameTime = 0f;
    protected float timer = 0f;

    private void Awake()
    {
        Physics.autoSimulation = false;

        perFrameTime = 1.0f / logicFramePerSecond;

        id2Input = new Dictionary<int, InputData>();

        InitConfigs();
        InitPlayers();
    }

    private void OnEnable()
    {
        gameInput.Enable();
    }

    private void OnDisable()
    {
        gameInput.Disable();
    }

    private void Update()
    {
        UpdateInput();//更新输入

        timer += Time.deltaTime;
        while (timer > perFrameTime)
        {
            timer -= perFrameTime;
            LogicUpdate(perFrameTime);
        }
    }

    private void LogicUpdate(float deltaTime)
    {
        foreach (var player in players)
        {
            player.LogicUpdate(deltaTime);
        }

        Physics.Simulate(deltaTime);
        ClearKey();//清理输入
    }

    private void InitPlayers()
    {
        foreach (var player in players)
        {
            player.Initialize(this);
        }
    }

    private void InitConfigs()
    {
        foreach (var asset in configAssets)
        {
            var config = DataUtility.FromJson<MachineConfig>(asset.text);
            configDict.Add(asset.name, config);
        }

        ActionMachineHelper.Init(LoadConfig);
    }

    private MachineConfig LoadConfig(string configName)
    {
        return configDict.TryGetValue(configName, out var config) ? config : null;
    }

    #region  IInputContainer

    private GameInput gameInput = null;
    private Dictionary<int, InputData> id2Input = new Dictionary<int, InputData>();
    public InputData lastInput { get; protected set; } = InputData.none;

    private float lookAtOffset;//0 为 Vector3.forward
    private float lookAtHeightScale;

    public float LookAtYAngle => MathUtility.FixedByteAngle(lookAtOffset);
    public float LookAtHeightScale => lookAtHeightScale;

    private void UpdateInput()
    {
        UpdateGlobalInputData();
        UpdateInputData();
    }

    private void UpdateGlobalInputData()
    {
        GameInput.PlayerActions playerInput = gameInput.Player;
        Vector2 lookDelta = playerInput.Look.ReadValue<Vector2>();
        if (Mathf.Abs(lookDelta.x) > 0.3f)
        {
            lookDelta.y = 0f;
        }

        lookAtOffset = (lookAtOffset + lookDelta.x) % 360f;
        lookAtHeightScale = Mathf.Clamp01(lookAtHeightScale - lookDelta.y * Time.deltaTime);
    }

    private void UpdateInputData()
    {
        GameInput.PlayerActions playerInput = gameInput.Player;

        InputData result = InputData.none;

        Vector2 move = playerInput.Move.ReadValue<Vector2>();
        move = Quaternion.AngleAxis(LookAtYAngle, Vector3.back) * move;

        ActionKeyCode keyCode = ActionKeyCode.None;

        if (move.x != 0)
        {
            keyCode |= move.x < 0 ? ActionKeyCode.Left : ActionKeyCode.Right;
        }

        if (move.y != 0)
        {
            keyCode |= move.y > 0 ? ActionKeyCode.Up : ActionKeyCode.Down;
        }

        if (playerInput.Move.phase == InputActionPhase.Started)
        {
            keyCode |= ActionKeyCode.Axis;
        }

        if (playerInput.Jump.triggered)
        {
            keyCode |= ActionKeyCode.Jump;
        }

        if (playerInput.Jump.phase == InputActionPhase.Started)
        {
            keyCode |= ActionKeyCode.Jumping;
        }

        if (playerInput.Attack.triggered)
        {
            keyCode |= ActionKeyCode.Attack;
        }

        if (playerInput.Attack.phase == InputActionPhase.Started)
        {
            keyCode |= ActionKeyCode.Attacking;
        }

        if (playerInput.Skill.triggered)
        {
            keyCode |= ActionKeyCode.Skill;
        }

        if (playerInput.Dash.triggered)
        {
            keyCode |= ActionKeyCode.Dash;
        }

        if (playerInput.Block.phase == InputActionPhase.Started)
        {
            keyCode |= ActionKeyCode.Blocking;
        }

        result.SetKeyCode(keyCode);
        result.SetAxisFromDir(move);

        AddInput(inputTargetId, result);
        lastInput = result;
    }

    public void ClearKey()
    {
        id2Input.Clear();
    }

    public void AddKey(int id, ActionKeyCode code)
    {
        if (id2Input.TryGetValue(id, out InputData data))
        {
            data.keyCode |= (int)code;
            id2Input[id] = data;
        }
        else
        {
            id2Input.Add(id, new InputData()
            {
                keyCode = (int)code,
                axisValue = 0
            });
        }
    }

    public void SetAxisFromDir(int id, bool axisState, Vector3 dir)
    {
        dir.y = 0f;
        float angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
        byte axisValue = MathUtility.AngleToByte(angle);
        SetAxis(id, axisState, axisValue);
    }

    public void SetAxis(int id, bool axisState, byte axisValue)
    {
        if (id2Input.TryGetValue(id, out InputData data))
        {
            data.keyCode = axisState ? (data.keyCode | (int)ActionKeyCode.Axis) : (data.keyCode & ~(int)ActionKeyCode.Axis);
            data.axisValue = axisState ? axisValue : byte.MaxValue;
            id2Input[id] = data;
        }
        else
        {
            id2Input.Add(id, axisState ? new InputData()
            {
                keyCode = (int)ActionKeyCode.Axis,
                axisValue = axisValue
            } : InputData.none);
        }
    }

    public InputData GetRawInput(int id)
    {
        return id2Input.TryGetValue(id, out InputData data) ? data : InputData.none;
    }

    public void AddInput(int id, InputData inputData)
    {
        if (id2Input.TryGetValue(id, out InputData data))
        {
            data.Append(inputData);
            id2Input[id] = data;
        }
        else
        {
            id2Input.Add(id, inputData);
        }
    }

    public int GetAllKey(int id)
    {
        if (!id2Input.TryGetValue(id, out InputData data))
        {
            return (int)ActionKeyCode.None;
        }

        return (int)data.keyCode;
    }

    public byte GetAxis(int id)
    {
        if (!id2Input.TryGetValue(id, out InputData data))
        {
            return 0;
        }

        return data.axisValue;
    }

    public bool HasKey(int id, int keyCode, bool fullMatch = false)
    {
        if (!id2Input.TryGetValue(id, out InputData data))
        {
            return false;
        }

        if (fullMatch)
        {
            return ((int)data.keyCode & keyCode) == keyCode;
        }
        else
        {
            return ((int)data.keyCode & keyCode) != 0;
        }
    }

    #endregion
}

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

        InitializeCamera();
    }

    private void OnEnable()
    {
        gameInput = new GameInput();
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

    private void LateUpdate()
    {
        UpdateFollowPos();
        UpdateTransform();
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


    #region Camera

    [Serializable]
    public class CameraSetting
    {
        //x = distance y = height z = xAngle

        public Vector3 minView = new Vector3(4, 2, 0);
        public Vector3 maxView = new Vector3(7, 4, 8);

        public Vector3 GetView(float v)
        {
            return Vector3.Lerp(minView, maxView, v);
        }
    }

    public CameraSetting cameraSetting;

    public new Camera camera { get; private set; }
    public Transform root { get; private set; }

    public Transform followObj { get; private set; }
    public Vector3 followPos { get; private set; }

    protected float yAngle => LookAtYAngle;
    protected float heightScale => LookAtHeightScale;
    protected float currentYAngle = 0f;

    protected float currentHeightScale = 0f;
    public void SetFollow(Transform obj)
    {
        followObj = obj;
        followPos = followObj.position;

        currentYAngle = yAngle;
        currentHeightScale = heightScale;
        UpdateTransform(followPos, currentYAngle, currentHeightScale);
    }

    public void InitializeCamera()
    {
        camera = Camera.main;

        GameObject rootObj = new GameObject("CameraRoot");
        root = rootObj.transform;
        camera.transform.parent = root;

        camera.transform.localScale = Vector3.one;
        camera.transform.localRotation = Quaternion.identity;
        camera.transform.localPosition = Vector3.zero;

        SetFollow(players.Find(t => t.ID == inputTargetId).transform);
    }

    private void UpdateTransform(Vector3 center, float yAngle, float viewValue)
    {
        Vector3 view = cameraSetting.GetView(viewValue);
        Vector3 dir = (Quaternion.AngleAxis(yAngle, Vector3.up) * Vector3.forward).normalized;
        Vector3 offset = dir * -view.x + Vector3.up * view.y;
        Vector3 targetPosition = center + offset;

        Quaternion targetRotation = Quaternion.Euler(view.z, yAngle, 0);

        root.position = targetPosition;
        root.rotation = targetRotation;

        DrawUtility.D.DrawLine(center, center + dir * 3f, Color.magenta);
    }

    private void UpdateTransform()
    {
        currentYAngle = Mathf.LerpAngle(currentYAngle, yAngle, Time.deltaTime * 7f);
        currentHeightScale = Mathf.LerpAngle(currentHeightScale, heightScale, Time.deltaTime * 16f);
        UpdateTransform(followPos, currentYAngle, currentHeightScale);
    }

    private void UpdateFollowPos()
    {
        if (followObj == null)
        {
            return;
        }
        followPos = followObj.position;
    }

    #endregion

    #region  Input

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
        byte axisValue = MathUtility.ByteAngleYFromDir(dir);
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

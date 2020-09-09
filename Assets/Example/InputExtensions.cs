/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 2020/8/13 2:10:43
 */

using UnityEngine;
using XMLib;
using XMLib.AM;

public static class IInputContainerExtensions
{
    public static float GetAngle(this IInputContainer container, int id)
    {
        InputData value = container.GetRawInput(id);
        return value.GetAngle();
    }

    public static Vector3 GetDir(this IInputContainer container, int id)
    {
        InputData value = container.GetRawInput(id);
        return value.GetDir();
    }
}

public static class InputDataExtensions
{
    public static void SetKeyCode(this ref InputData data, ActionKeyCode keyCode)
    {
        data.keyCode = (int)keyCode;
    }

    public static void Append(this ref InputData data, InputData other)
    {
        data.keyCode |= other.keyCode;
        data.axisValue = other.axisValue;

        if ((other.keyCode & (int)ActionKeyCode.Axis) == 0)
        {//矫正，如果最后没有输入摇杆，则去除已存的摇杆状态，否者axisValue可能被误使用，导致方向错误
            data.keyCode &= ~(int)ActionKeyCode.Axis;
            data.axisValue = byte.MaxValue;
        }
    }

    /// <summary>
    /// 去除单帧的指令
    /// </summary>
    /// <param name="data"></param>
    public static void RemoveOnceKeyCode(this ref InputData data)
    {
        data.keyCode &= (int)~(ActionKeyCode.Attack | ActionKeyCode.Dash | ActionKeyCode.Jump | ActionKeyCode.Skill);
    }

    public static void SetAxisFromDir(this ref InputData data, Vector2 dir)
    {
        float angle = dir == Vector2.zero ? 0f : Vector2.SignedAngle(dir, Vector2.up);
        data.axisValue = MathUtility.AngleToByte(angle);
    }

    public static ActionKeyCode GetKeyCode(this ref InputData data)
    {
        return (ActionKeyCode)data.keyCode;
    }

    public static float GetAngle(this ref InputData data)
    {
        return MathUtility.ByteToAngle(data.axisValue);
    }

    public static Vector3 GetDir(this ref InputData data)
    {
        float angle = data.GetAngle();
        return (Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward).normalized;
    }

    public static Quaternion GetRotation(this ref InputData data)
    {
        float angle = data.GetAngle();
        return Quaternion.AngleAxis(angle, Vector3.up);
    }
}
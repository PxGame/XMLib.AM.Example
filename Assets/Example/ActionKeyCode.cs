/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 2019/11/18 18:10:51
 */

using System;

/// <summary>
/// ActionKeyCode
/// </summary>
[Flags]
public enum ActionKeyCode
{
    None = 0b0000,

    Up = 0x1,
    Down = 0x2,
    Left = 0x4,
    Right = 0x8,

    Attack = 0x10,
    Jump = 0x20,
    Jumping = 0x40,
    Dash = 0x80,

    Blocking = 0x100,
    Attacking = 0x200,
    Skill = 0x400,
    Axis = 0x800,
}
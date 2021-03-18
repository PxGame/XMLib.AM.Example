/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 1/3/2019 2:50:29 PM
 */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XMLib;
using XMLib.Extensions;

/// <summary>
/// 摇杆按钮
/// </summary>
[AddComponentMenu("XMLib/UI/Joystick Button")]
[RequireComponent(typeof(CanvasGroup))]
public class JoystickButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    /// <summary>
    /// 状态
    /// </summary>
    private enum Status
    {
        Down,
        Up,
        Drag,
        Reset
    }

    [Serializable] public class ButtonClickedEvent : UnityEvent { }

    [Serializable] public class ButtonUpEvent : UnityEvent { }

    [Serializable] public class ButtonDownEvent : UnityEvent { }

    [Serializable] public class ButtonAxisEvent : UnityEvent<Vector2> { }

    [SerializeField] protected RectTransform normalPos;

    [SerializeField] protected float normalAlpha = 0.5f;

    [SerializeField] protected float _radius = 50f;

    [SerializeField] protected RectTransform _handler;

    [SerializeField] protected RectTransform _background;

    [SerializeField] private ButtonClickedEvent _onClick;

    [SerializeField] private ButtonClickedEvent _onUp = null;

    [SerializeField] private ButtonClickedEvent _onDown = null;

    [SerializeField] private ButtonAxisEvent _onAxis;

    private CanvasGroup _canvasGroup;
    private Vector2 _centerPosition;
    private Status _currentState = Status.Reset;

    /// <summary>
    /// 按下事件
    /// </summary>
    public ButtonClickedEvent onClick
    {
        get { return _onClick; }
        set { _onClick = value; }
    }

    /// <summary>
    /// 摇杆值事件
    /// </summary>
    public ButtonAxisEvent onAxis
    {
        get { return _onAxis; }
        set { _onAxis = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = normalPos != null ? normalAlpha : 0f;
        _centerPosition = normalPos != null ? normalPos.GetCenterLocalPosition() : _centerPosition;
        _background.SetCenterLocalPosition(_centerPosition);
        _handler.SetCenterLocalPosition(_centerPosition);
    }

    protected override void OnEnable()
    {
        UpdateAxis(Status.Reset, Vector2.zero);
    }

    protected override void OnDisable()
    {
        UpdateAxis(Status.Reset, Vector2.zero);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press();
    }

    private void Press()
    {
        if (!IsActive())
            return;

        OnClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        _onDown.Invoke();

        //
        Vector2 point = eventData.position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, point, eventData.pressEventCamera, out point);
        UpdateAxis(Status.Down, point);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        _onUp.Invoke();

        //
        Vector2 point = eventData.position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, point, eventData.pressEventCamera, out point);
        UpdateAxis(Status.Up, point);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        Vector2 point = eventData.position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, point, eventData.pressEventCamera, out point);
        UpdateAxis(Status.Drag, point);
    }

    /// <summary>
    /// 更新摇杆
    /// </summary>
    private void UpdateAxis(Status status, Vector2 position)
    {
        if (_currentState == Status.Reset && status != Status.Down)
        {//忽略
            return;
        }

        Status oldStatue = _currentState;
        _currentState = status;

        Vector2 handlerPosition;
        if (_currentState == Status.Drag)
        {
            _canvasGroup.alpha = 1f;
            handlerPosition = position;
        }
        else if (_currentState == Status.Down)
        {
            _canvasGroup.alpha = 1f;
            handlerPosition = position;

            _centerPosition = position;
            _background.SetCenterLocalPosition(position);
        }
        else
        {//Status.Reset,Status.Up
            _canvasGroup.alpha = normalPos != null ? normalAlpha : 0f;
            _centerPosition = normalPos != null ? normalPos.GetCenterLocalPosition() : _centerPosition;
            _background.SetCenterLocalPosition(_centerPosition);
            handlerPosition = _centerPosition;
        }

        Vector2 offset = (Vector2)handlerPosition - _centerPosition;
        Vector2 normal = offset.normalized;
        float length = offset.magnitude;

        //校验
        if (length > _radius)
        {//限制半径
            length = _radius;

            offset = length * normal;
            handlerPosition = _centerPosition + offset;
        }

        //设置坐标
        _handler.SetCenterLocalPosition(handlerPosition);

        Vector2 value = offset / _radius;

        //设置值

        _onAxis.Invoke(value);
    }

    #region 事件

    private void OnClick()
    {
        _onClick.Invoke();
    }

    #endregion 事件
}
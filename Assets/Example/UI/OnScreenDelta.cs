/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/10/22 10:58:55
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using XMLib;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace AliveCell
{
    /// <summary>
    /// OnScreenAxis
    /// </summary>
    [AddComponentMenu("Input/On-Screen Delta")]
    public class OnScreenDelta : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }

        private Vector2 m_lastPosition = Vector2.zero;
        private RectTransform m_rt;

        private void Start()
        {
            m_rt = transform.GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out var position);

            Vector2 delta = position - m_lastPosition;
            SendValueToControl(delta);
            m_lastPosition = position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out var position);
            m_lastPosition = position;
            SendValueToControl(Vector2.zero);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SendValueToControl(Vector2.zero);
        }
    }
}
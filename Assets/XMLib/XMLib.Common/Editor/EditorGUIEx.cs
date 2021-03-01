/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/1/13 22:53:37
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// EditorGUIEx
    /// </summary>
    public static class EditorGUIEx
    {
        #region MinMaxSlider

        public const float minMaxThumbWidth = 5f;

        private static readonly int _MinMaxSliderHash = "XMLib.EditorMinMaxSlider".GetHashCode ();

        public static bool MinMaxSlider (Rect position, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            return DoMinMaxSlider (EditorGUI.IndentedRect (position), GUIUtility.GetControlID (_MinMaxSliderHash, FocusType.Passive), ref minValue, ref maxValue, minLimit, maxLimit);
        }

        private static bool DoMinMaxSlider (Rect position, int id, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            float size = maxValue - minValue;
            EditorGUI.BeginChangeCheck ();
            bool result = _DoMinMaxSlider (position, id, ref minValue, ref size, minLimit, maxLimit);
            GUI.changed = true;
            if (EditorGUI.EndChangeCheck ())
            {
                maxValue = minValue + size;
            }
            return result;
        }

        class MinMaxSliderStateEx
        {
            public float dragStartPos = 0; // Start of the drag (mousePosition)
            public float dragStartValue = 0; // Value at start of drag.
            public float dragStartSize = 0; // Size at start of drag.
            public float dragStartValuesPerPixel = 0;
            public float dragStartLimit = 0; // start limit at start of drag
            public float dragEndLimit = 0; // end limit at start of drag
            public int whereWeDrag = -1; // which part are we dragging? 0 = middle, 1 = min, 2 = max, 3 = min trough, 4 = max trough
        }

        static MinMaxSliderStateEx _stateEx;

        private static bool _DoMinMaxSlider (Rect position, int id, ref float value, ref float size, float minLimit, float maxLimit)
        {
            Event evt = Event.current;

            //校验
            minLimit = Mathf.Min (minLimit, maxLimit);
            maxLimit = Mathf.Max (minLimit, maxLimit);
            value = Mathf.Clamp (value, minLimit, maxLimit);
            size = Mathf.Clamp (size + value, value, maxLimit) - value;

            //范围
            float mousePosition = evt.mousePosition.x - position.x - minMaxThumbWidth;
            float pixelsPerValue = (position.width - (minMaxThumbWidth * 2)) / (maxLimit - minLimit);
            Rect vaildRt = new Rect (
                position.x + minMaxThumbWidth,
                position.y,
                position.width - 2 * minMaxThumbWidth,
                position.height
            );
            Rect thumbCenter = new Rect (
                (value - minLimit) * pixelsPerValue + vaildRt.x,
                vaildRt.y,
                size * pixelsPerValue,
                vaildRt.height
            );
            Rect thumbMin = new Rect (thumbCenter.x - minMaxThumbWidth, thumbCenter.y, minMaxThumbWidth, thumbCenter.height);
            Rect thumbMax = new Rect (thumbCenter.x + thumbCenter.width, thumbCenter.y, minMaxThumbWidth, thumbCenter.height);
            Rect thumbRt = new Rect (
                thumbCenter.x - minMaxThumbWidth,
                thumbCenter.y,
                thumbCenter.width + minMaxThumbWidth * 2,
                thumbCenter.height
            );

            //

            switch (evt.GetTypeForControl (id))
            {
                case EventType.MouseDown:
                    if (evt.button != 0 || !position.Contains (evt.mousePosition) || minLimit - maxLimit == 0)
                    {
                        return false;
                    }

                    if (_stateEx == null)
                    {
                        _stateEx = new MinMaxSliderStateEx ();
                    }

                    _stateEx.dragStartLimit = minLimit;
                    _stateEx.dragEndLimit = maxLimit;

                    if (thumbRt.Contains (evt.mousePosition))
                    {
                        _stateEx.dragStartPos = mousePosition;
                        _stateEx.dragStartValue = value;
                        _stateEx.dragStartSize = size;
                        _stateEx.dragStartValuesPerPixel = pixelsPerValue;
                        _stateEx.whereWeDrag = thumbMin.Contains (evt.mousePosition) ? 1 : thumbMax.Contains (evt.mousePosition) ? 2 : 0;

                        GUIUtility.hotControl = id;
                        evt.Use ();

                        return true;
                    }

                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        evt.Use ();
                        GUIUtility.hotControl = 0;
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != id)
                    {
                        return false;
                    }

                    float deltaVal = (mousePosition - _stateEx.dragStartPos) / _stateEx.dragStartValuesPerPixel;
                    switch (_stateEx.whereWeDrag)
                    {
                        case 0: // normal drag
                            value = Mathf.Clamp (_stateEx.dragStartValue + deltaVal, minLimit, maxLimit - size);
                            break;
                        case 1: // min size drag
                            value = _stateEx.dragStartValue + deltaVal;
                            size = _stateEx.dragStartSize - deltaVal;
                            if (value < minLimit)
                            {
                                size -= minLimit - value;
                                value = minLimit;
                            }
                            if (size < 0)
                            {
                                value -= 0 - size;
                                size = 0;
                            }
                            break;
                        case 2: // max size drag
                            size = _stateEx.dragStartSize + deltaVal;
                            if (value + size > maxLimit)
                                size = maxLimit - value;
                            if (size < 0)
                                size = 0;
                            break;
                    }

                    GUI.changed = true;
                    evt.Use ();
                    break;
                case EventType.Repaint:
                    //GUI.skin.box.Draw(position, GUIContent.none, id);
                    GUI.skin.button.Draw (thumbCenter, GUIContent.none, id);
                    GUI.skin.button.Draw (thumbMin, GUIContent.none, id);
                    GUI.skin.button.Draw (thumbMax, GUIContent.none, id);

                    EditorGUIUtility.AddCursorRect (thumbMin, MouseCursor.ResizeHorizontal, _stateEx != null && _stateEx.whereWeDrag == 1 ? id : -1);
                    EditorGUIUtility.AddCursorRect (thumbMax, MouseCursor.ResizeHorizontal, _stateEx != null && _stateEx.whereWeDrag == 2 ? id : -1);

                    break;
            }

            return false;
        }

        #endregion
    }
}

/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/29 10:38:40
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// EditorGUIUtilityEx
    /// </summary>
    public static class EditorGUILayoutEx
    {
        public static List<T> DragAndDropBox<T>(string text, params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            string[] paths = DragAndDropBox($"{text}", options);
            List<T> results = new List<T>();

            for (int i = 0; i < paths.Length; i++)
            {
                T obj = AssetDatabase.LoadAssetAtPath<T>(paths[i]);
                if (obj != null)
                {
                    results.Add(obj);
                }
            }

            return results;
        }

        public static string[] GetRectDragAndDrop(Rect rect)
        {
            Event evt = Event.current;

            string[] results = null;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (rect.Contains(evt.mousePosition))
                    {
                        evt.Use();
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    }
                    break;

                case EventType.DragExited:
                    if (rect.Contains(evt.mousePosition))
                    {
                        evt.Use();
                        results = DragAndDrop.paths;
                    }
                    break;
            }

            return results ?? Array.Empty<string>();
        }

        public static string[] DragAndDropBox(string text, params GUILayoutOption[] options)
        {
            Rect rect = EditorGUILayout.GetControlRect(options);
            string[] results = GetRectDragAndDrop(rect);
            GUI.Box(rect, text, "groupbox");
            return results;
        }

        public delegate void ItemDrawerCallback<T>(int index, ref bool selected, T obj) where T : class;

        public static int DrawList<T>(IList<T> list, int selectIndex, ref Vector2 scrollPos, Action<Action<T>> adder, ItemDrawerCallback<T> itemDrawer) where T : class
        {
            ItemDrawerCallback<T> drawer = itemDrawer ?? DefaultDrawer;

            CheckSelectIndex(ref selectIndex, list);

            bool isAdd = false;
            bool isRemove = false;

            using (var sv = new GUILayout.ScrollViewScope(scrollPos))
            {
                using (var v2 = new GUILayout.VerticalScope())
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        T obj = list[i];
                        bool selected = selectIndex == i;
                        bool oldSelected = selected;

                        using (var h3 = new GUILayout.HorizontalScope())
                        {
                            drawer(i, ref selected, obj);
                        }
                        GUILayout.Space(2);

                        if (selected)
                        {
                            selectIndex = i;
                        }
                        else if (oldSelected)
                        {
                            selectIndex = -1;
                        }
                    }
                    GUILayout.FlexibleSpace();
                }

                scrollPos = sv.scrollPosition;
            }

            using (var v = new GUILayout.VerticalScope("IN Footer"))
            {
                if (selectIndex != -1)
                {
                    using (var h2 = new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("上移") && selectIndex - 1 >= 0)
                        {
                            GUI.FocusControl(null);
                            T wrapObj = list[selectIndex];
                            list[selectIndex] = list[selectIndex - 1];
                            list[selectIndex - 1] = wrapObj;
                            selectIndex -= 1;
                        }
                        if (GUILayout.Button("下移") && selectIndex + 1 < list.Count)
                        {
                            GUI.FocusControl(null);
                            T wrapObj = list[selectIndex];
                            list[selectIndex] = list[selectIndex + 1];
                            list[selectIndex + 1] = wrapObj;
                            selectIndex += 1;
                        }

                        if (GUILayout.Button("不选"))
                        {
                            GUI.FocusControl(null);
                            selectIndex = -1;
                        }
                    }
                }

                using (var h2 = new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("添加"))
                    {
                        GUI.FocusControl(null);
                        isAdd = true;
                    }
                    if (GUILayout.Button("删除"))
                    {
                        GUI.FocusControl(null);
                        isRemove = true;
                    }
                }
            }

            if (isAdd)
            {//必须放在外面，否则GUILayout会报错
                adder((t) =>
                {
                    if (t == null)
                    {
                        return;
                    }

                    list.Add(t);
                    selectIndex = list.Count - 1;
                });
            }

            if (isRemove)
            {
                if (selectIndex >= 0)
                {
                    list.RemoveAt(selectIndex);
                    CheckSelectIndex(ref selectIndex, list);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请选择一项", "确定");
                }
            }
            return selectIndex;

            int CheckSelectIndex(ref int index, IList<T> target)
            {
                return index = list == null ? -1 : Mathf.Clamp(index, -1, list.Count - 1);
            }

            void DefaultDrawer(int index, ref bool selected, T obj)
            {
                if (GUILayout.Button($"{index}", selected ? "MeTransitionSelectHead" : "MeTransitionSelect", GUILayout.ExpandHeight(true), GUILayout.Width(15)))
                {
                    GUI.FocusControl(null);
                    selected = !selected;
                }
                if (GUILayout.Button($"{obj}", GUI.skin.label, GUILayout.Height(30f), GUILayout.ExpandWidth(true)))
                {
                    GUI.FocusControl(null);
                    selected = !selected;
                }
            }
        }

        private static object DrawObjectWithTypes(GUIContent title, object obj, Type[] types, object[] attrs)
        {
            if (types == null || types.Length == 0)
            {
                return null;
            }

            Type type = obj?.GetType();
            int index = 0;

            if (obj == null
            || type == null
            || (type != null && 0 > (index = Array.FindIndex(types, t => t == type))))
            {
                if (obj != null && type != null && (index = Array.FindIndex(types, t => t.ConvertToChecker(type))) >= 0)
                {//转换类型
                    type = types[index];
                    obj = Convert.ChangeType(obj, type);
                }
                else
                {//初始化类型
                    index = 0;
                    type = types[index];
                    obj = TypeUtility.CreateInstance(type);
                }
            }

            using (var v = new EditorGUILayout.VerticalScope())
            {
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = CalcLabelWidth(title);
                int targetIndex = EditorGUILayout.Popup(title, index, types.Select(t => t.GetSimpleName()).ToArray());
                EditorGUIUtility.labelWidth = oldLabelWidth;

                if (targetIndex != index)
                {
                    type = types[targetIndex];
                    index = targetIndex;
                    try
                    {
                        obj = Convert.ChangeType(obj, type);
                    }
                    catch (Exception)
                    {
                        obj = type == typeof(string) ? string.Empty : Activator.CreateInstance(type);
                    }
                }
                obj = DrawObject(new GUIContent($"{type.GetSimpleName()}"), obj, type, attrs);
            }

            return obj;
        }

        private static IList DrawObjectsWithTypes(GUIContent title, IList objs, Type objsType, Type[] types, object[] attrs)
        {
            if (!objsType.IsGenericType || objsType.GenericTypeArguments.Length != 1)
            {
                EditorGUILayout.LabelField($"不支持 {objsType} 类型");
                return objs;
            }
            Type baseType = objsType.GenericTypeArguments[0];

            if (objs == null)
            {//初始化变量
                objs = (IList)TypeUtility.CreateInstance(objsType);
            }

            using (var lay = new EditorGUILayout.VerticalScope("FrameBox"))
            {
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = CalcLabelWidth(title);
                EditorGUILayout.BeginHorizontal();
                int cnt = EditorGUILayout.IntField(title, objs.Count);
                if (GUILayout.Button("+", GUILayout.Width(40)))
                {
                    cnt++;
                    GUI.FocusControl(null);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = oldLabelWidth;

                int diff = cnt - objs.Count;

                while (diff < 0)
                {
                    objs.RemoveAt(objs.Count - 1);
                    diff++;
                }

                while (diff > 0)
                {
                    object subObj = TypeUtility.CreateInstance(types[0]);
                    objs.Add(subObj);
                    diff--;
                }

                EditorGUI.indentLevel += 1;
                for (int i = 0; i < cnt; i++)
                {
                    using (var lay2 = new EditorGUILayout.VerticalScope("FrameBox"))
                    {
                        objs[i] = DrawObjectWithTypes(new GUIContent($"{i}"), objs[i], types, attrs);

                        using (var lay3 = new EditorGUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("↑") && i > 0)
                            {
                                object swap = objs[i - 1];
                                objs[i - 1] = objs[i];
                                objs[i] = swap;
                            }
                            if (GUILayout.Button("↓") && i < cnt - 1)
                            {
                                object swap = objs[i + 1];
                                objs[i + 1] = objs[i];
                                objs[i] = swap;
                            }
                            if (GUILayout.Button("x"))
                            {
                                objs.RemoveAt(i);
                                i--;
                                cnt--;
                            }
                        }
                    }
                }
                EditorGUI.indentLevel -= 1;
            }

            return objs;
        }

        public static float CalcLabelWidth(GUIContent label)
        {
            return GUI.skin.label.CalcSize(label).x + EditorGUI.indentLevel * GUI.skin.label.fontSize * 2;
        }

        public static float CalcLabelWidth(string label)
        {
            return GUI.skin.label.CalcSize(new GUIContent(label)).x + EditorGUI.indentLevel * GUI.skin.label.fontSize * 2;
        }

        private static object DrawObjectCustom(out bool isDraw, GUIContent title, object obj, Type type, object[] attrs = null)
        {
            try
            {
                MethodInfo method = TypeCache.GetMethodsWithAttribute<ObjectDrawerAttribute>().FirstOrDefault(t =>
                {
                    if (!t.IsStatic)
                    {
                        return false;
                    }
                    ObjectDrawerAttribute od = t.GetCustomAttribute<ObjectDrawerAttribute>();
                    if (od == null || od.type != type)
                    {
                        return false;
                    }
                    return true;
                });

                isDraw = method != null;
                if (!isDraw)
                {
                    return obj;
                }

                obj = method.Invoke(null, new object[] { title, obj, type, attrs });
                isDraw = true;
                return obj;
            }
            catch
            {
                isDraw = false;
                return obj;
            }
        }

        public static object DrawObject(GUIContent title, object obj, Type type, object[] attrs = null)
        {
            using (var lay = new EditorGUILayout.VerticalScope())
            {
                if (null == obj)
                {//初始化
                    obj = TypeUtility.CreateInstance(type);
                }

                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = CalcLabelWidth(title);

                obj = DrawObjectCustom(out bool isDraw, title, obj, type, attrs);
                if (!isDraw)
                {
                    switch (obj)
                    {
                        case float v:
                            {
                                RangeAttribute attr = GetTargetAttr<RangeAttribute>();
                                obj = null == attr ? EditorGUILayout.FloatField(title, v) : EditorGUILayout.Slider(title, v, attr.min, attr.max);
                            }
                            break;

                        case double v:
                            obj = EditorGUILayout.DoubleField(title, v);
                            break;

                        case int v:
                            {
                                RangeAttribute attr = GetTargetAttr<RangeAttribute>();
                                obj = null == attr ? EditorGUILayout.IntField(title, v) : EditorGUILayout.IntSlider(title, v, (int)attr.min, (int)attr.max);
                            }
                            break;

                        case string v:
                            obj = EditorGUILayout.TextField(title, v);
                            break;

                        case bool v:
                            obj = EditorGUILayout.Toggle(title, v);
                            break;

                        case Vector2 v:
                            {
                                v = EditorGUILayout.Vector2Field(title, v);
                                RangeAttribute attr = GetTargetAttr<RangeAttribute>();
                                if (attr != null)
                                {
                                    v.x = Mathf.Clamp(v.x, attr.min, attr.max);
                                    v.y = Mathf.Clamp(v.y, attr.min, attr.max);
                                    if (v.x > v.y)
                                    {
                                        v.x = v.y;
                                    }

                                    using (var lay2 = new EditorGUILayout.HorizontalScope())
                                    {
                                        GUILayout.Space(EditorGUIUtility.labelWidth);
                                        EditorGUILayout.MinMaxSlider(ref v.x, ref v.y, attr.min, attr.max);
                                    }
                                }
                                obj = v;
                            }
                            break;

                        case Vector3 v:
                            obj = EditorGUILayout.Vector3Field(title, v);
                            break;

                        case Vector2Int v:
                            {
                                v = EditorGUILayout.Vector2IntField(title, v);
                                RangeAttribute attr = GetTargetAttr<RangeAttribute>();
                                if (attr != null)
                                {
                                    Vector2 vf = v;

                                    v.x = Mathf.Clamp(Mathf.RoundToInt(vf.x), (int)attr.min, (int)attr.max);
                                    v.y = Mathf.Clamp(Mathf.RoundToInt(vf.y), (int)attr.min, (int)attr.max);
                                    if (v.x > v.y)
                                    {
                                        v.x = v.y;
                                    }

                                    using (var lay2 = new EditorGUILayout.HorizontalScope())
                                    {
                                        GUILayout.Space(EditorGUIUtility.labelWidth);
                                        EditorGUILayout.MinMaxSlider(ref vf.x, ref vf.y, attr.min, attr.max);
                                    }

                                    v = new Vector2Int((int)vf.x, (int)vf.y);
                                }
                                obj = v;
                            }
                            break;

                        case Vector3Int v:
                            obj = EditorGUILayout.Vector3IntField(title, v);
                            break;

                        case Enum v:
                            {
                                FlagsAttribute attr = GetTargetAttr<FlagsAttribute>();
                                obj = null == attr ? EditorGUILayout.EnumPopup(title, v) : EditorGUILayout.EnumFlagsField(title, v);
                            }
                            break;

                        case LayerMask v:
                            {
                                int mask = InternalEditorUtility.LayerMaskToConcatenatedLayersMask(v);
                                mask = EditorGUILayout.MaskField(title, mask, InternalEditorUtility.layers);
                                obj = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(mask);
                            }
                            break;

                        case IList v:
                            {
                                if (type.IsGenericType && type.GenericTypeArguments.Length == 1)
                                {
                                    Type subType = type.GenericTypeArguments[0];

                                    using (var lay2 = new EditorGUILayout.VerticalScope("FrameBox"))
                                    {
                                        if (subType.IsClass || subType.IsValueType)
                                        {
                                            EditorGUIUtility.labelWidth = CalcLabelWidth(title);
                                            EditorGUILayout.BeginHorizontal();
                                            int cnt = EditorGUILayout.IntField(title, v.Count);
                                            if (GUILayout.Button("+", GUILayout.Width(40)))
                                            {
                                                cnt++;
                                                GUI.FocusControl(null);
                                            }
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUIUtility.labelWidth = oldLabelWidth;

                                            int diff = cnt - v.Count;

                                            while (diff < 0)
                                            {
                                                v.RemoveAt(v.Count - 1);
                                                diff++;
                                            }

                                            while (diff > 0)
                                            {
                                                object subObj = TypeUtility.CreateInstance(subType);
                                                v.Add(subObj);
                                                diff--;
                                            }

                                            EditorGUI.indentLevel += 1;
                                            for (int i = 0; i < cnt; i++)
                                            {
                                                using (var lay3 = new EditorGUILayout.VerticalScope("FrameBox"))
                                                {
                                                    v[i] = DrawObject(new GUIContent($"{i}"), v[i], subType, attrs);

                                                    using (var lay4 = new EditorGUILayout.HorizontalScope())
                                                    {
                                                        if (GUILayout.Button("↑") && i > 0)
                                                        {
                                                            object swap = v[i - 1];
                                                            v[i - 1] = v[i];
                                                            v[i] = swap;
                                                        }
                                                        if (GUILayout.Button("↓") && i < cnt - 1)
                                                        {
                                                            object swap = v[i + 1];
                                                            v[i + 1] = v[i];
                                                            v[i] = swap;
                                                        }
                                                        if (GUILayout.Button("x"))
                                                        {
                                                            v.RemoveAt(i);
                                                            i--;
                                                            cnt--;
                                                        }
                                                    }
                                                }
                                            }
                                            EditorGUI.indentLevel -= 1;
                                        }
                                    }
                                }
                                else
                                {
                                    EditorGUILayout.LabelField($"不支持 {type} 类型");
                                }

                                obj = v;
                            }
                            break;

                        default:
                            {
                                if (!type.IsPrimitive && (type.IsClass || type.IsValueType))
                                {
                                    FieldInfo[] fields = type.GetFields();

                                    //=============================================================
                                    //查找成员显示控制
                                    Dictionary<string, bool> enableDict = EnableToggleAttribute.GetEnableDict(obj);
                                    //=============================================================

                                    int depth = 0;
                                    if (title != GUIContent.none)
                                    {
                                        EditorGUILayout.LabelField($"{title.text}");
                                        depth = 1;
                                    }

                                    EditorGUI.indentLevel += depth;
                                    foreach (var field in fields)
                                    {
                                        //=============================================================
                                        if (!EnableToggleItemAttribute.EnableChecker(field, enableDict))
                                        {//不显示
                                            continue;
                                        }
                                        //===========================================================

                                        DrawField(obj, field);
                                    }
                                    EditorGUI.indentLevel -= depth;
                                }
                                else
                                {
                                    EditorGUILayout.LabelField($"不支持 {type} 类型");
                                }
                            }
                            break;
                    }
                }
                EditorGUIUtility.labelWidth = oldLabelWidth;

                return obj;

                T GetTargetAttr<T>() where T : Attribute
                {
                    T result = null;
                    if (attrs != null)
                    {
                        result = (T)Array.Find(attrs, t => t is T);
                    }

                    if (result == null)
                    {
                        result = type.GetCustomAttribute<T>();
                    }
                    return result;
                }
            }
        }

        public static void DrawField(object target, FieldInfo fieldInfo)
        {
            object fieldValue = fieldInfo.GetValue(target);

            ObjectTypesAttribute attr = fieldInfo.GetCustomAttribute<ObjectTypesAttribute>();
            object[] attrs = fieldInfo.GetCustomAttributes(true);

            GUIContent title = new GUIContent(fieldInfo.Name);
            if (attr == null)
            {
                fieldValue = DrawObject(title, fieldValue, fieldInfo.FieldType, attrs);
            }
            else if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
            {
                fieldValue = DrawObjectsWithTypes(title, (IList)fieldValue, fieldInfo.FieldType, attr.types, attrs);
            }
            else
            {
                fieldValue = DrawObjectWithTypes(title, fieldValue, attr.types, attrs);
            }

            fieldInfo.SetValue(target, fieldValue);
        }

        public static bool MinMaxSlider(ref float minValue, ref float maxValue, float minLimit, float maxLimit, params GUILayoutOption[] options)
        {
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth / 2, EditorGUIUtility.singleLineHeight, GUI.skin.horizontalSlider);
            return EditorGUIEx.MinMaxSlider(position, ref minValue, ref maxValue, minLimit, maxLimit);
        }

        #region Extensions

        public static object DrawObject(string title, object obj, Type type, object[] attrs = null)
        {
            return DrawObject(new GUIContent(title), obj, type, attrs);
        }

        public static T DrawObject<T>(GUIContent title, T obj)
        {
            return (T)DrawObject(title, obj, typeof(T));
        }

        public static T DrawObject<T>(string title, T obj)
        {
            return (T)DrawObject(title, obj, typeof(T));
        }

        #endregion Extensions
    }
}
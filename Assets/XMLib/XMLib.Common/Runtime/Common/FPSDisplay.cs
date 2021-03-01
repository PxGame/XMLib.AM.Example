/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/10/20 12:21:29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// FPSDisplay
    /// </summary>
    public class FPSDisplay : MonoBehaviour
    {
        public bool dontDestoryOnLoad = false;

        public bool showTop;

        [Range(0, 1)]
        public float textScale = 1f;

        public TextAnchor textAnchor = TextAnchor.MiddleCenter;

        public Color textColor = new Color(0, 0, 1, 0.5f);
        public Color backgroundColor = new Color(0, 0, 0, 0.5f);

        private float _deltaTime = 0.0f;

        private void Awake()
        {
            if (dontDestoryOnLoad) DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        private GUIStyle style;

        private void OnEnable()
        {
            style = new GUIStyle();
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
            int charH = (int)(Mathf.Min(w, h) * textScale);

            style.fontSize = charH;
            style.normal.textColor = textColor;
            style.alignment = textAnchor;

            Rect rect = new Rect(0, showTop ? 0 : (h - charH), w, charH);

            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, Mathf.Round(fps));

            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, backgroundColor, 0, 0);
            GUI.Label(rect, text, style);
        }
    }
}
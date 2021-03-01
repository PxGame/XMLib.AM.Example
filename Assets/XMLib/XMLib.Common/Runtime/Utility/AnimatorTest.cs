/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/7 15:32:14
 */

using UnityEngine;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace XMLib
{
    /// <summary>
    /// AnimatorTest
    /// </summary>
    public class AnimatorTest : MonoBehaviour
    {
        public Animator animator;

        private void OnValidate()
        {
            if (animator != null)
            {
                return;
            }

            animator = GetComponent<Animator>();
            if (animator != null)
            {
                return;
            }

            animator = GetComponentInChildren<Animator>();
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(AnimatorTest))]
    public class AnimatorTestEditor : Editor
    {
        private float _time = 0f;
        private int _selectIndex = 0;
        private bool _autoUpdate = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Animator animator = ((AnimatorTest)target).animator;
            if (null == animator)
            {
                EditorGUILayout.HelpBox("请设置目标Animator", UnityEditor.MessageType.Warning);
                return;
            }

            var clips = animator.runtimeAnimatorController.animationClips;

            if (clips.Length == 0)
            {
                EditorGUILayout.HelpBox("Animator 中没有动画片段", UnityEditor.MessageType.Warning);
                return;
            }
            _selectIndex = Mathf.Clamp(_selectIndex, 0, clips.Length);
            string[] clipNames = clips.Select(t => t.name).ToArray();

            _selectIndex = EditorGUILayout.Popup("动画片段", _selectIndex, clipNames);
            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("应用"))
                {
                    string clipName = clipNames[_selectIndex];
                    animator.CrossFadeInFixedTime(clipName, 0.1f);
                }
            }
            else
            {
                AnimationClip clip = clips[_selectIndex];

                //_time = Mathf.Clamp(_time, 0, clip.length);
                _time = EditorGUILayout.Slider($"进度[{clip.length}]", _time, 0, clip.length);

                if (GUILayout.Button("应用") || _autoUpdate)
                {
                    clip.SampleAnimation(animator.gameObject, _time);
                }

                _autoUpdate = EditorGUILayout.Toggle("自动应用", _autoUpdate);
            }
        }
    }

#endif
}
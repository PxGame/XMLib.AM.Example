/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/12 14:36:09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;

namespace XMLib
{
    /// <summary>
    /// EditorUtility
    /// </summary>
    public static class EditorUtilityEx
    {
        /// <summary>
        /// 获取选择的文件夹
        /// </summary>
        /// <returns></returns>
        public static string GetSelectDirectory()
        {
            string[] strs = Selection.assetGUIDs;
            if (strs.Length == 0)
            {
                return "Assets";// string.Empty;
            }

            string resourceDirectory = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (string.IsNullOrEmpty(resourceDirectory) || !Directory.Exists(resourceDirectory))
            {
                return "Assets";// string.Empty;
            }

            return resourceDirectory;
        }

        /// <summary>
        /// 获取选择的文件
        /// </summary>
        /// <param name="suffixFilter"></param>
        /// <returns></returns>
        public static string GetSelectFile(params string[] suffixFilter)
        {
            string[] strs = Selection.assetGUIDs;
            if (strs.Length == 0)
            {
                return string.Empty;
            }

            string resourceFile = AssetDatabase.GUIDToAssetPath(strs[0]);
            if (!File.Exists(resourceFile))
            {
                return string.Empty;
            }

            string suffix = Path.GetExtension(resourceFile);
            if (suffixFilter.Length > 0 && Array.Exists(suffixFilter, t => 0 == string.Compare(t, suffix, true)))
            {
                return resourceFile;
            }

            return string.Empty;
        }

        public static void OpenFolder(string folderPath)
        {
            if (!Path.IsPathRooted(folderPath))
            {
                folderPath = Path.Combine(UnityEngine.Application.dataPath, "..", folderPath).Replace("\\", "/");
            }
            string path = $"file://{folderPath}";
            UnityEngine.Application.OpenURL(path);
        }

        public static string ValidFilePath(string filePath)
        {
            int index = 0;
            string target = filePath;
            do
            {
                if (!File.Exists(target))
                {
                    return target;
                }

                string ext = Path.GetExtension(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string dir = Path.GetDirectoryName(filePath).Replace('\\', '/');

                index++;
                target = $"{dir}/{fileName}_{index}{ext}";
            }
            while (true);
        }

        public static List<string> FindAssetsToGUID(string dir, string findFilter)
        {
            string[] findPaths = new string[] { dir };
            string[] guids = AssetDatabase.FindAssets(findFilter, findPaths);
            return new List<string>(guids);
        }

        public static List<string> FindAssetsToPath(string dir, string findFilter)
        {
            List<string> paths = FindAssetsToGUID(dir, findFilter);

            //转换成path
            for (int i = 0; i < paths.Count; i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            }

            return paths;
        }

        public static List<T> FindAssets<T>(string dir, string findFilter) where T : UnityEngine.Object
        {
            List<string> paths = FindAssetsToPath(dir, findFilter);
            List<T> results = new List<T>();

            foreach (var path in paths)
            {
                T obj = AssetDatabase.LoadAssetAtPath<T>(path);
                if (obj != null)
                {
                    results.Add(obj);
                }
            }

            return results;
        }

        public static bool OpenScene(string scenePath, OpenSceneMode mode, out Scene scene)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                scene = new Scene();
                return false;
            }

            scene = EditorSceneManager.OpenScene(scenePath, mode);

            return true;
        }

        public static bool OpenScene(SceneAsset sceneAsset, OpenSceneMode mode, out Scene scene)
        {
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            return OpenScene(scenePath, mode, out scene);
        }
    }
}
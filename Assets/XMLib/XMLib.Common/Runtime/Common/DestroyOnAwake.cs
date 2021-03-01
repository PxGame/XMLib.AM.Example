/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/11/20 23:56:26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLib;

namespace XMLib
{
    /// <summary>
    /// DestroyOnAwake
    /// </summary>
    public class DestroyOnAwake : MonoBehaviour
    {
        protected void Awake()
        {
            Destroy(gameObject);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 13.2装的是原本胶囊体的
    /// </summary>
    [Serializable]
    public class DefaultColliderData
    {
        //默认数据
        //模型高度
        [field: SerializeField] public float Height { get; private set; } = 1.8f;
        //中心取高度一半
        [field: SerializeField] public float CenterY { get; private set; } = 0.9f;
        //半径
        [field: SerializeField] public float Radius { get; private set; } = 0.2f;
    }
}

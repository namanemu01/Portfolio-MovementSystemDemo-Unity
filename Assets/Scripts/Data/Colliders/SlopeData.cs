using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    [Serializable]//可以让自定义类在检查窗口看到的特性
    /// <summary>
    /// 13.4斜率数据
    /// </summary>
    public class SlopeData 
    {
        //步长高度百分比,这个头是表示使属性可以显示在检查窗口的
        [field: SerializeField][field: Range(0f, 1f)] public float StepHeightPersentage { get; private set; } = 0.25f;
        //其他的属性将会在需要的时候补充

        //浮动射线距离 也就是设置好的高度
        [field: SerializeField][field: Range(0f, 5f)] public float FloatRayDistance { get; private set; } = 2f;
        //升力
        [field: SerializeField][field: Range(0f, 50f)] public float StepReachForce { get; private set; } = 25f;
    }
}

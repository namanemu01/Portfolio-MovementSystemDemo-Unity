using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    //这个类是被PlayerGroundedStateSO引用的，要想序列化必须加上这个属性
    //它和SO的关系是：当你想让你的自定义类（如PlayerGroundedData）作为一个字段，
    //在SO（或MonoBehaviour、或其他可序列化类）的Inspector面板中显示和编辑时，这个自定义类就必须加 [Serializable]。
    [Serializable]
    /// <summary>
    /// 被so引用的 地面数据
    /// </summary>
    public class PlayerGroundedData 
    {
        //根本目的：[SerializeField] 和 [Serializable] 的根本目的是 “纳入Unity序列化系统”。
        //直接表现：因为被Unity序列化系统管理了，所以它才能在Inspector窗口中被看见、被编辑。
        //所以，让窗口看见 是 被序列化 的一个必然结果和可视化表现，而不是最终目的。最终目的是数据的持久化。

        //基础移动速度
        [field: SerializeField][field:Range(0f,25f)] public float BaseSpeed { get; private set; } = 5f;
        //斜坡速度曲线
        [field: SerializeField] public AnimationCurve SlopeSpeedAngles { get; private set; }
        //基础旋转数据脚本
        [field: SerializeField] public PlayerRotationData BaseRotationData { get; private set; }
        //行走数据脚本
        [field: SerializeField] public PlayerWalkData WalkData { get; private set; }
        //奔跑数据脚本
        [field: SerializeField] public PlayerRunData RunData { get; private set; }
        [field: SerializeField] public PlayerDashData DashData { get; private set; }
    }
}

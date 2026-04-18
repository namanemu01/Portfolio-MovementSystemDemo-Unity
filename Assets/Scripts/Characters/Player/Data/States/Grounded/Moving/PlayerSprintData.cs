using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    [Serializable]
    public class PlayerSprintData 
    {
        //设置冲刺速度修改器的范围，1.7f是根据动画的实际速度调整的
        [field: SerializeField][field: Range(1f,3f)]public float SpeedModifier { get; private set; } = 1.7f;
        //设置了进入冲刺状态的时间为1秒，就是按住一秒钟。
        [field: SerializeField][field: Range(0f, 5f)] public float SprintToRunTime { get; private set; } = 1f;
        //这里是从冲刺状态过渡到走，需要经过的跑状态的持续时间，所以写到冲刺数据里
        [field: SerializeField][field: Range(0f, 2f)] public float RunToWalkTime { get; private set; } = 0.5f;
    }
}

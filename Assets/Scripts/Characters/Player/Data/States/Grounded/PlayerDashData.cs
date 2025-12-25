using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    [Serializable]
    public class PlayerDashData 
    {
        //15.3添加速度修改器 并将此数据表存入对应的 地面状态数据表中
        [field: SerializeField][field:Range(1f,3f)]public float SpeedModifier { get; private set; } = 2f;
        //被视为连续的时间
        [field: SerializeField][field: Range(0f, 2f)] public float TimeToBeConsideredConsecutive { get; private set; } = 1f;
        //连续冲刺限制数量
        [field: SerializeField][field: Range(1,10)] public int ConsecutiveDashesLimitAmount { get; private set; } = 2;
        //冲刺限制到达的冷却时间
        [field: SerializeField][field: Range(0f, 5f)] public float DashLimitReachedCooldown { get; private set; } = 1.75f;
    }
}

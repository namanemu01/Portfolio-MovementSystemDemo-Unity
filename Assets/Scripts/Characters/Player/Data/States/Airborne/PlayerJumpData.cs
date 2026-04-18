using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    [Serializable]
    public class PlayerJumpData
    {
        [field: SerializeField] public PlayerRotationData RotationData { get; private set; }
        [field: SerializeField][field: Range(0f, 5f)] public float JumpToGroundRayDistance { get; private set; } = 2f;
       
        //在这里添加两个动画曲线 用于调整玩家在斜坡上跳跃时的跳跃力
        [field: SerializeField] public AnimationCurve JumpForceModifierOnSlopeUpwards { get; private set; }
        [field: SerializeField] public AnimationCurve JumpForceModifierOnSlopeDownwards { get; private set; }


        //在这里为每种力强度增加一个属性 以便于在编辑器中调整
        //这个汉译为静止力
        [field:SerializeField]public Vector3 StationaryForce { get; private set; }
        [field: SerializeField] public Vector3 WeakForce { get; private set; }
        [field: SerializeField] public Vector3 MediumForce { get; private set; }
        [field: SerializeField] public Vector3 StrongForce { get; private set; }

        [field: SerializeField][field: Range(0f, 10f)] public float DecelerationForce { get; private set; } = 1.5f;
    }
}

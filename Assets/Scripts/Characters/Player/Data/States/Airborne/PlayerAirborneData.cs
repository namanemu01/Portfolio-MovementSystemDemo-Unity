using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 负责管理玩家在空中状态的数据，如跳跃数据等
    /// </summary>
    [Serializable]
    public class PlayerAirborneData
    {
        //这类管理跳跃类数据 以便于在编辑器中调整
        [field: SerializeField] public PlayerJumpData JumpData { get; private set; }

        [field: SerializeField] public PlayerFallData FallData { get; private set; }
    }
}

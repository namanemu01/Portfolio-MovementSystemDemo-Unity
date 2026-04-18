using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 16.3这个类用于显示玩家的胶囊体碰撞器工具类，
    /// 
    /// </summary>
    [Serializable]
    public class PlayerCapsuleColliderUtility : CapsuleColliderUtility
    {
        //触发器碰撞器数据
        [field: SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }
        //19.4
        protected override void OnInitialize()
        {
            base.OnInitialize();
            TriggerColliderData.Initialize();
        }
    }
}

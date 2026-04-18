using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 用来处理一些很小的缝隙碰撞器不合适进入状态 所以使用盒型碰撞器避免
    /// </summary>
    [Serializable]
    public class PlayerTriggerColliderData
    {
        [field: SerializeField]public BoxCollider GroundCheckCollider {get; private set;}

        //19.4 防止高处掉落状态不对的bug'
        public Vector3 GroundCheckColliderExtents { get;private set;}

        public void Initialize()
        {
            GroundCheckColliderExtents = GroundCheckCollider.bounds.extents;
        }
    }
}

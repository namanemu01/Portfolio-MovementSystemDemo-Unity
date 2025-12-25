using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    [Serializable]
    public class PlayerLayerData
    {
        //LayerMask是专门用来指定射线检测生效层级的结构体 可限定射线仅与特定层级的物体发生碰撞检测
        //也就是他会在检查器生成一个栏 然后你选择可以被射线检测到的层级类别
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }
    }
}

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

        /// <summary>
        /// 检查layermask是否包含某个layer 也就是检测图层比较
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool ContainsLayer(LayerMask layerMask,int layer)
        {
            //15.2里面讲了
            //以前有学过那个layer是三十二个二进制数
            //layermask位掩码用计算机表示，layer用阿拉伯数表示，要想让他们比较就得改一下
            //layermask那个是向开关一样，就用位移挪走就可以了
            //如果是0就不包含
            return (1 << layer & layerMask) != 0;
        }

        /// <summary>
        /// 检查是不是地面层
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool  IsGroundLayer(int layer)
        {
            return ContainsLayer(GroundLayer, layer);
        }
    }
}

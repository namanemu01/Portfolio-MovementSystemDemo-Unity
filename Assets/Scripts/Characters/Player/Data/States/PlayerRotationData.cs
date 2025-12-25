using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    [Serializable]
    public class PlayerRotationData
    { 
        //脚本对象数据
        [field:SerializeField]public Vector3 TargetRotationReachTime {  get; private set; }
    }
}

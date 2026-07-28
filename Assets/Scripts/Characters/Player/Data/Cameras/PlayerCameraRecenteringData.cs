using System;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 负责玩家相机重新定位的数据
    /// </summary>
    [Serializable]
    public class PlayerCameraRecenteringData 
    {
        //最大
        [field: SerializeField] [field:Range(0f,360f)]public float MinimumAngle { get; private set; }
        //最小
        [field: SerializeField][field: Range(0f, 360f)] public float MaximumAngle { get; private set; }
        //等待时间 也就是玩家没有输入的时间超过这个值 就会开始重新定位
        [field: SerializeField][field: Range(-1f, 20f)] public float WaitTime { get; private set; }
        //重新定位的时间 也就是从玩家没有输入到相机重新定位完成的时间
        [field: SerializeField][field: Range(-1f, 20f)] public float RecenteringTime { get; private set; }

        /// <summary>
        /// 判断是否在角度范围内 也就是玩家输入的角度是否在最小和最大之间 如果在范围内 就会重新定位
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public bool IsWithinRange(float angle)
        {
            return angle >= MinimumAngle && angle <= MaximumAngle;
        }
    }
}

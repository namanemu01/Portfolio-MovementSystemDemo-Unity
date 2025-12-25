using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 保存需要在多个状态间重用的数据，如是否应该走路shouldWalk，movementInput玩家移动输入等
    /// </summary>
    public class PlayerStateReuseableData
    {
        //12.2为此处所有的属性提供一个公共集合以便于修改数据

        //玩家运动输入，之前不是在movementstate里面单独声明了一个变量吗，现在给替换了
        public Vector2 MovementInput { get; set; }
        //运动速度修改器
        public float MovementSpeedModifier { get; set; } = 1f;
        //斜坡上运动速度修改器 因为默认是1 所以直接赋值1
        public float MovementOnSlopesSpeedModifier { get; set; } = 1f;
        //进入走路状态的标志
        public bool ShouldWalk { get; set; } = false;

        //*当属性类型为引用类型时（如类、接口、数组、委托等），属性存储的是对实际数据的引用，而不是数据本身。
        //*当属性类型为值类型时（如int、float、struct等），属性存储的是实际的数据值，如果从属性获取值，会得到数据的副本。
        //所以我们要换一种写法:(最好的写法就是set）
        private Vector3 currentTargrtRotation;
        private Vector3 timeToReachTargetRotation;
        private Vector3 dampedTargetRotationCurrentVelocity;
        private Vector3 dampedTargetRotationPassedTime;

        /// <summary>
        /// 我们需要自己设置一个引用类型的属性， 获取 并 返回 以便在多个状态之间共享和修改 当前目标旋转数据
        /// </summary>
        public ref Vector3 CurrentTargetRotation
        {
            get
            {
                return ref currentTargrtRotation;
            }
        }
        public ref Vector3 TimeToReachTargetRotation
        {
            get
            {
                return ref timeToReachTargetRotation;
            }
        }
        public ref Vector3 DampedTargetRotationCurrentVelocity
        {
            get
            {
                return ref dampedTargetRotationCurrentVelocity;
            }
        }
        public ref Vector3 DampedTargetRotationPassedTime
        {
            get
            {
                return ref dampedTargetRotationPassedTime;
            }
        }

    }
}

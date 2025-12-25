using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 第九节 空闲状态脚本 啥也不干
    /// </summary>
    public class PlayerIdlingState : PlayerGroundedState
    {
        //这几个是要被缓存的状态，因为频繁切换
        public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods 接口_状态方法
        /// <summary>
        /// 9.2进入状态 方法
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            //1.一旦进入这个状态，将速度修改器设置为0，这样保证进来这个状态就不再移动
            //1.不需要离开这个值再设置，因为每个进入状态都会重新设置这个修改器的值
            stateMachine.ResuableData.MovementSpeedModifier = 0f;

            //2.重置玩家速度，以防玩家因为物理原因移动
            ResetVelocity();
        }

        /// <summary>
        /// 9.3更新方法 在这里切换 其他状态
        /// </summary>
        public override void Update()
        {
            base.Update();
            //如果输入为0 调回
            if (stateMachine.ResuableData.MovementInput == Vector2.zero) return;

            //否则 进入切换状态
            OnMove();
        }

        
        #endregion
    }
}

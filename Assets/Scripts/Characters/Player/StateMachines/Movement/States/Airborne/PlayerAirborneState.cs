using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    public class PlayerAirborneState : PlayerMovementState
    {
        public PlayerAirborneState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods

        public override void Enter()
        {
            //这样可以使每次进入空降状态 冲刺都会设为false 这样就不需要在每个空降状态里都写一次了
            //不希望跳跃状态出现这样所以那边另设定
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.AirborneParemeterHash);

            ResetSprintState();
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.AirborneParemeterHash);

        }
        #endregion

        #region Reusable Methods 可重用方法
        /// <summary>
        /// 碰撞器接触地面时 操作
        /// </summary>
        /// <param name="collider"></param>
        protected override void OnContactWithGround(Collider collider)
        {
            //从空中接触到地面 变成轻着陆状态
            stateMachine.ChangeState(stateMachine.LightLandingState);
        }

        protected virtual void ResetSprintState()
        {
            stateMachine.ReusableData.ShouldSprint = false;
        }
        #endregion
    }
}

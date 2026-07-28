using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    //13.1继承停止状态类 这是一个轻度停止状态 也就是玩家在地面上没有输入的时候的状态
    public class PlayerLightStoppingState : PlayerStoppingState
    {
        public PlayerLightStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            stateMachine.ReusableData.MovementDecelerationForce = movementData.StopData.LightDecelerationForce;

            //15
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;
        }
        #endregion
    }
}

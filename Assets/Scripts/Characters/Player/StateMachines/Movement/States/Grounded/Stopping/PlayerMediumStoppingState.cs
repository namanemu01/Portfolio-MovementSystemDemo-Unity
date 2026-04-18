using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    //13.2
    public class PlayerMediumStoppingState : PlayerStoppingState
    {
        public PlayerMediumStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.MediumStopParemeterHash);

            stateMachine.ReusableData.MovementDecelerationForce = movementData.StopData.MediumDecelerationForce;

            //15
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.MediumStopParemeterHash);
        }
        #endregion
    }
}

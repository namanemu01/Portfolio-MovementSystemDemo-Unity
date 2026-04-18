using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    public class PlayerLandingState : PlayerGroundedState
    {
        public PlayerLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        //1.ÏÈÐ´´Ó»¬Ïè×´Ì¬¹ý¶Éµ½×ÅÂ½×´Ì¬ 
        #region IState Methods
        public override void Enter()
        {
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.LandingParemeterHash);
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.LandingParemeterHash);
        }
        #endregion

    }
}

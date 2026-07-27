using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    public class PlayerSprintingState : PlayerMovingState
    {
        private PlayerSprintData sprintData;

        private float startTime;

        private bool keepSprinting;
        private bool shouldResetSprintState;
        public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }
        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = sprintData.SpeedModifier;

            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.SprintParemeterHash);

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

            shouldResetSprintState = true;

            startTime = Time.time;
        }
        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.SprintParemeterHash);
           
            if(shouldResetSprintState)
            {
                keepSprinting = false;
                stateMachine.ReusableData.ShouldSprint = false;
            }
        }
        public override void Update()
        {
            base.Update();
            if(keepSprinting)
            {
                return;
            }
            if(Time.time < startTime + sprintData.SprintToRunTime)
            {
                return;
            }
            StopSprinting();
        }

        #endregion
        #region Main Methods
        private void StopSprinting()
        {
           if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.HardStoppingState);

                return;
            }
            stateMachine.ChangeState(stateMachine.RunningState);
        }

        #endregion
        #region Reusable Methods
        protected override void AddInputActionsCallBacks()
        {
            base.AddInputActionsCallBacks();

            // Sprint.performed 已在 PlayerGroundedState 中订阅，这里不需要重复订阅
        }
        protected override void RemoveInputActionsCallBacks()
        {
            base.RemoveInputActionsCallBacks();

            // 对应上面的空实现，无需额外移除
        }
        protected override void OnFall()
        {
            shouldResetSprintState = false;

            base.OnFall();
        }
        #endregion
        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.HardStoppingState);

            base.OnMovementCanceled(context);
        }
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
            shouldResetSprintState = false;

            base.OnJumpStarted(context);
        }
        /// <summary>
        /// ��ס�㹻��ʱ��Ż����״̬
        /// </summary>
        /// <param name="contect"></param>
        protected override void OnSprintPerformed(InputAction.CallbackContext contect)
        {
            keepSprinting = true;

            stateMachine.ReusableData.ShouldSprint = true;
        }
        #endregion

    }
}

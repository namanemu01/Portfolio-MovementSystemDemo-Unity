using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    public class PlayerRunningState : PlayerMovingState
    {
        private PlayerSprintData sprintData;

        private float startTime;
        public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }
        #region IState Methods接口方法
        /// <summary>
        /// 10.1先设置速度修改器 先写进入逻辑
        /// </summary>
        public override void Enter()
        {
            //其实在做的是把状态的逻辑和数据分离为脚本和so文件
            stateMachine.ReusableData.MovementSpeedModifier = movementData.RunData.SpeedModifier;

            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.RunParemeterHash);

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;

            startTime = Time.time;
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.RunParemeterHash);
        }
        public override void Update()
        {
            base.Update();
            if(!stateMachine.ReusableData.ShouldWalk)
            {
                return;
            }
            if(Time.time > startTime + sprintData.RunToWalkTime)
            {
                return;
            }
            StopRunning();
        }

        //10.2想 跑步状态和什么状态有关联，把walk的重用和输入方法复制过来
        #endregion

        //#region Reusable Methods 可重用方法
        ///// <summary>
        ///// 9.5给另一个状态添加一个回调
        ///// </summary>
        //protected override void AddInputActionsCallBacks()
        //{
        //    //9.5添加移动和取消操作
        //    base.AddInputActionsCallBacks();

        //    stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;
        //}


        //protected override void RemoveInputActionsCallBacks()
        //{
        //    base.RemoveInputActionsCallBacks();

        //    stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;
        //}
        //#endregion
        #region Main Methods
        private void StopRunning()
        {
            if(stateMachine.ReusableData.MovementInput==Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.MediumStoppingState);

                return;
            }
            stateMachine.ChangeState(stateMachine.WalkingState);
        }
        #endregion
        #region 输入方法 Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.MediumStoppingState);
            base.OnMovementCanceled(context);
        }
        /// <summary>
        /// 9.4给每个状态都回调，这个不用，直接切换，这个不用回调是因为直接切换状态了
        /// </summary>
        /// <param name="context"></param>
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);

            if (stateMachine.ReusableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);
            }
        }

        ///// <summary>
        ///// 逐渐理解了这里的输入回调，是为了监听按键的松开，避免其他状态每次都要去检测输入
        ///// </summary>
        ///// <param name="context"></param>
        //protected void OnMovementCanceled(InputAction.CallbackContext context)
        //{
        //    //松开按键切换到待机状态
        //    stateMachine.ChangeState(stateMachine.IdlingState);
        //}
        //因为已经添加了playergroundedstate的回调 所以这里不需要再添加了
        #endregion
    }
}

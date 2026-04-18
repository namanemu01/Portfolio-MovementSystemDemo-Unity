using System;
using System.Collections;
using System.Collections.Generic;
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

            //15
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;
            //
            startTime = Time.time;
        
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.RunParemeterHash);
        }
        //因为有了疾跑状态，现在来修改从步行到跑步到步行的逻辑
        public override void Update()
        {
            base.Update();
            //意思是疾跑完了应该跑步再进入走路
            if(!stateMachine.ReusableData.ShouldWalk)
            {
                return;
            }
            //
            if(Time.time<startTime+sprintData.RunToWalkTime)
            {
                return;
            }
            //
            StopRunning();
        }


        //10.2想 跑步状态和什么状态有关联，把walk的重用和输入方法复制过来
        #endregion

        #region Main Methods

        private void StopRunning()
        {
            //转换为中停止状态，什么也不输入就待机
            if(stateMachine.ReusableData.MovementInput==Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdlingState);
                return;
            }
            //如果不是无输入，就进入走路状态
            stateMachine.ChangeState(stateMachine.WalkingState);
        }
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

        #region 输入方法 Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            //这里重写了，原来的逻辑是进待机状态，现在有了停止状态就覆盖了
            stateMachine.ChangeState(stateMachine.MediumStoppingState);

            base.OnMovementCanceled(context);
        }

        /// <summary>
        /// 9.4给每个状态都回调，这个不用，直接切换，这个不用回调是因为直接切换状态了
        /// </summary>
        /// <param name="context"></param>
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            //我觉得调用这个代表是先修改shouldwalk的状态，如果已经是walk了，取消，然后进入跑步状态
            //这个调用了shouldwalk
            base.OnWalkToggleStarted(context);
            //过渡状态,唯一修改的就是这里 从跑步切换到走路
            stateMachine.ChangeState(stateMachine.WalkingState);
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
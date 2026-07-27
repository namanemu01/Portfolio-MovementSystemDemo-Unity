using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    /// <summary>
    /// 这里面 一个是监听松开键进入待机状态 另一个是从走路切换到跑步状态
    /// </summary>
    public class PlayerWalkingState : PlayerMovingState
    {
        private PlayerWalkData walkData;
        public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            walkData = movementData.WalkData;
        }

        #region IState Methods
        /// <summary>
        /// 9.4首先先写进入逻辑 
        /// </summary>
        public override void Enter()
        {
            //更改速度控制器
            //12.4 更改变量 数据交换
            stateMachine.ReusableData.MovementSpeedModifier = walkData.SpeedModifier;

            stateMachine.ReusableData.BackwardsCameraRecenteringData = walkData.BackwardsCameraRecenteringData;

            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.WalkParemeterHash);

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.WalkParemeterHash);
            
            SetBaseCameraRecenteringData();
        }
        #endregion


        #region 输入方法 Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.LightStoppingState);

            base.OnMovementCanceled(context);
        }
        /// <summary>
        /// 9.4给每个状态都回调，这个不用，直接切换
        /// </summary>
        /// <param name="context"></param>
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);

            if (!stateMachine.ReusableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.RunningState);
            }
        }


        //protected void OnMovementCanceled(InputAction.CallbackContext context)
        //{
        //    //松开按键切换到待机状态
        //    stateMachine.ChangeState(stateMachine.IdlingState);
        //}
        #endregion

        //以上是步行状态的全部逻辑

        // 实际发生的过程：

//[玩家松开方向键]
//    ↓
//[输入系统检测到Movement.canceled事件]
//    ↓
//[查找所有监听这个事件的方法]
//    ↓
//[调用 OnMovementCanceled(context)]  ← 系统自动调用！
//    ↓
//[你的方法执行：stateMachine.ChangeState(...)]
//    ↓
//[新状态的Enter()，旧状态的Exit()]
//    ↓
//[Exit()中执行：Movement.canceled -= OnMovementCanceled]
    }
}

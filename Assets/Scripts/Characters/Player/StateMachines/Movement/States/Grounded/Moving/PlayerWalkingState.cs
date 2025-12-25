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
        public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        /// <summary>
        /// 9.4首先先写进入逻辑 
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            //更改速度控制器
            //12.4 更改变量 数据交换
            stateMachine.ResuableData.MovementSpeedModifier = movementData.WalkData.SpeedModifier;
        }
        #endregion


        #region 输入方法 Input Methods
        /// <summary>
        /// 9.4给每个状态都回调，这个不用，直接切换
        /// </summary>
        /// <param name="context"></param>
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            //我觉得调用这个代表是先修改shouldwalk的状态，如果已经是walk了，取消，然后进入跑步状态
            //这个调用了shouldwalk
            base.OnWalkToggleStarted(context);
            //过渡状态
            stateMachine.ChangeState(stateMachine.RunningState);
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

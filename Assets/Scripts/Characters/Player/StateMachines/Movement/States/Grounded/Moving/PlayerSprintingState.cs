using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    public class PlayerSprintingState : PlayerMovingState
    {
        //保存冲刺数据变量,在这里方便引用
        private PlayerSprintData sprintData;
        //进入的时间
        private float stratTime;
        //要写出那种长按几秒进入冲刺状态，如果没有达到要求就进入疾跑状态的情况 需要写新的变量
        private bool keepSprinting;

        //15.7
        private bool shouldResetSprintState;
        public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }

        //首先设置此状态的速度调节器
        #region IState Methods
        public override void Enter()
        { 
            //设置速度调节器 通过总类运动状态链接地面数据脚本
            stateMachine.ReusableData.MovementSpeedModifier = sprintData.SpeedModifier;

            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.SprintParemeterHash);

            //15
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

            //15.7
            shouldResetSprintState = true;

            //设置开始时间，第一个变量
            stratTime = Time.time;
        }
        //为了避免下次进入还是冲刺，最后一步所以需要把这次退出状态之后变量改回初始值
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.SprintParemeterHash);
            if (shouldResetSprintState)
            {
               keepSprinting = false;
               stateMachine.ReusableData.ShouldSprint = false;
            }
            //诸如这个逻辑我们也可以从疾跑状态改成跳跃状态
        }
        public override void Update()
        {
            base.Update();
            //如果说是true,说明已经在冲刺状态。就保持这种状态
            if (keepSprinting)
            {
                return;
            }
            //如果这种情况，说明还没有足够时间进入跑步状态
            //里面的条件说的是如果当前时间小于进入这个状态时间加上跑步状态的时间
            if (Time.time < stratTime + sprintData.SprintToRunTime)
            {
                return;
            }
            //写的是重点方法讲的是进入了但是退出的情况
            StopSprinting();
        }
        #endregion
        #region Main Methods

        //在这里转换跑步状态或者硬停止（目前么有硬停止，所以是待机）状态
        private void StopSprinting()
        {
            //首先如果没有输入防止进入此方法，没有输入的时候就待机
            if (stateMachine.ReusableData.MovementInput==Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdlingState);
                return;
            }
            //如果有输入从冲刺切换到跑步状态
            stateMachine.ChangeState(stateMachine.RunningState);
        }
        #endregion
        #region Reusable Methods
        //添加回调
        protected override void AddInputActionsCallBacks()
        {
            base.AddInputActionsCallBacks();
            //添加新的回调
            stateMachine.Player.Input.PlayerActions.Sprint.performed += OnSprintPerformed;
        }

        //移除
        protected override void RemoveInputActionsCallBacks()
        {
            base.RemoveInputActionsCallBacks();
            //减去新的回调
            stateMachine.Player.Input.PlayerActions.Sprint.performed -= OnSprintPerformed;
        }
        #endregion
        #region Input Methods

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            //这里重写了，原来的逻辑是进待机状态，现在有了停止状态就覆盖了
            stateMachine.ChangeState(stateMachine.HardStoppingState);

            base.OnMovementCanceled(context);
        }

        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
            //在调用父类方法之前先把这个变量改成false，防止进入跳跃状态之后还保持冲刺状态
            shouldResetSprintState = false;
            base.OnJumpStarted(context);
        }
        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            //默认是false，如果达到时长就改成true添加状态切换成冲刺，如果没有key达到就是写成
            keepSprinting = true;

            //15.7在这里添加一个变量来判断是否需要进入冲刺状态
            //在这里添加只是因为冲刺键需要在持续足够时间的情况下才进入冲刺状态，所以在这里添加这个变量来判断是否需要进入冲刺状态
            stateMachine.ReusableData.ShouldSprint = true;
        }
        
        protected override void OnFall()
        {
            //在调用父类方法之前先把这个变量改成false，防止进入下落状态之后还保持冲刺状态
            shouldResetSprintState = false;
            //转换成下落状态
            base.OnFall();
        }
        #endregion
    }
}

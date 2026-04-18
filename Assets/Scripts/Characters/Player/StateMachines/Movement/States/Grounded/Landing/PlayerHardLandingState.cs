using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    public class PlayerHardLandingState : PlayerLandingState
    {
        public PlayerHardLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        //1.进入时 逻辑
        #region IState Methods
        //进入之后一样 不能移动 并且重置在着陆之前的速度
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.HardLandParemeterHash);
            //进入这个状态时 不能移动 只能等着落地动画播完才能切换到下一个状态
            //相对的 离开状态可以移动
            stateMachine.Player.Input.PlayerActions.Movement.Disable();


            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.HardLandParemeterHash);

            //离开状态时 可以移动了
            stateMachine.Player.Input.PlayerActions.Movement.Enable();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!IsMovingHorizontally())
            {
                return;
            }
            ResetVelocity();
        }

        public override void OnAnimationExitEvent()
        {
            //动画结束时 也就是着陆动画播完时 可以移动了
            stateMachine.Player.Input.PlayerActions.Movement.Enable();
        }

        //2.悟了 每个状态就写切换到下一个状态的逻辑 不用管上一个过来的状态是什么了
        //只有坠落状态才会进入这个状态 
        //首先要了解要设计什么样的条件来切换到这个状态 以及从这个状态切换到什么状态
        //硬着陆可以冲刺 不能慢走
        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }


        #endregion
        #region Reusable Methods
        //因为有限制条件 所以需要重写这个方法 让它在硬着陆状态时不能使用慢走 只能使用冲刺和疾跑等 几帧不能移动

        //复习一下回调是具体操作了什么
        protected override void AddInputActionsCallBacks()
        {
            base.AddInputActionsCallBacks();

            stateMachine.Player.Input.PlayerActions.Movement.started += OnMovementStarted;
        }

        protected override void RemoveInputActionsCallBacks()
        {
            base.RemoveInputActionsCallBacks();

            stateMachine.Player.Input.PlayerActions.Movement.started -= OnMovementStarted;
        }

        
        protected override void OnMove()
        {
            //如果满足慢走条件 就直接返回 不执行下面的切换状态逻辑
            if (stateMachine.ReusableData.ShouldWalk)
            {
                return;
            }
            stateMachine.ChangeState(stateMachine.RunningState);
        }
        #endregion

        #region Input Methods
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
            //硬着陆状态不能跳跃 留空意味不会执行跳跃
        }
        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            //可以移动 疾走 冲刺
            OnMove();
        }
        #endregion
    }
}

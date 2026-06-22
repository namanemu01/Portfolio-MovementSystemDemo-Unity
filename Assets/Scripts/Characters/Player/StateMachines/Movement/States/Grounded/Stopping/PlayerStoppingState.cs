using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    //第十三步，这是停止状态中的一个管理类 包括其他三个状态被继承
    public class PlayerStoppingState : PlayerGroundedState
    {
        //要塑造 停止按键之后 慢慢减速最后停止 的状态
        public PlayerStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        #region IState Methods
        override public void Enter()
        {
            //进入这个状态的时候 先把速度设置为0
            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            SetBaseCameraRecenteringData();

            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.StoppingParemeterHash);

        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.StoppingParemeterHash);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            //14旋转 因为之前找摄像机的时候已经写过了 这里直接用就行了
            RotateTowardsTargetRotation();

            //在这个状态里 速度慢慢减小 最后停下来
            //进行检查和减速
            if(!IsMovingHorizontally())
            {
                //true 说明没有移动
                return;
            }
            DecelerateHorizontally();
        }
        //重写动画过渡事件方法 这里是写从停止状态过渡到其他状态的逻辑 因为现在是从停止状态进入待机状态 这个过渡事件就是在动画机里写的 从停止动画过渡到待机动画的事件
        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallBacks()
        {
            base.AddInputActionsCallBacks();

            stateMachine.Player.Input.PlayerActions.Movement.started += OnMovementStarted;
        }


        protected override void RemoveInputActionsCallBacks()
        {
            base.RemoveInputActionsCallBacks();
            stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementStarted;
        }
        #endregion

        #region Input Methods
        //在这个状态里 取消输入不进入待机状态 因为有了停止状态 取消输入应该进入停止状态 而不是待机状态
       
            //设置空是因为停止之后会进入待机 导致移动取消这个回调无用 
            //因为这个方法原本内容就是切换到 待机状态 就重复了，所以直接把它设置成空就好了
        

        
        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            //负责转移到移动状态
            OnMove();
        }
        #endregion
    }
}

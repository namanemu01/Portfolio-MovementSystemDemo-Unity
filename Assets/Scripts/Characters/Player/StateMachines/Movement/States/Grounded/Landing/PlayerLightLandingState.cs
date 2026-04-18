using UnityEngine;

namespace MovementStstem
{
    public class PlayerLightLandingState : PlayerLandingState
    {
        public PlayerLightLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        //2.进入时 逻辑
        #region IState Methods
        public override void Enter()
        {

            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            base.Enter();

           
            //17 设置在轻着陆状态下 可以使用跳跃 
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;

            ResetVelocity();
        }

        //4.切换移动状态 要在更新方法设计 用来切换到移动状态
        public override void Update()
        {
            //因为通过跳跃 滑翔过来的
            base.Update();

            if (stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                return;
            }

            OnMove();
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

        //3.切换不移动的状态 用动画 过渡事件 可以从着陆转到待机状态
        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }
        #endregion
    }
}

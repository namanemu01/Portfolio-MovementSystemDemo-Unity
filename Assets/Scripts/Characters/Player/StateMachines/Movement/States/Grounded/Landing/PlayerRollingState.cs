using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    public class PlayerRollingState : PlayerLandingState
    {
        private PlayerRollData rollData;

        public PlayerRollingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            rollData = movementData.RollData;
        }

        #region IState Methods
        public override void Enter()
        {
            //停止按移动键 也会保持滚动
            stateMachine.ReusableData.MovementSpeedModifier = rollData.SpeedModifier;

            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.RollParemeterHash);

            //滚动之后 不能冲刺
            stateMachine.ReusableData.ShouldSprint = false; 
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.RollParemeterHash);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            //如果有输入 就返回  没输入 继续旋转
            //确保在不调用Move的时候 进行旋转
            if(stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }
            RotateTowardsTargetRotation();
        }
        /// <summary>
        /// 设置动画转换事件
        /// </summary>
        public override void OnAnimationTransitionEvent()
        {
            //下来了如果没有移动 中停止，移动了 再考虑
            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.MediumStoppingState);

                return;
            }
            //如果移动 冲刺已经false了 所以不用覆盖move
            OnMove();
        }
        #endregion

        #region Input Methods
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
           //留空是不希望能跳
        }
        #endregion
    }
}

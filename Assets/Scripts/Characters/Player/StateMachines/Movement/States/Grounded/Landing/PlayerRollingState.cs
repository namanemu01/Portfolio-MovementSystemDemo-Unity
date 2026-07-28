using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    public class PlayerRollingState : PlayerLandingState
    {
        private PlayerRollData rollData;

        private float startTime;

        public PlayerRollingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            rollData = movementData.RollData;
        }

        #region IState Methods
        public override void Enter()
        {
            //ֹͣ���ƶ��� Ҳ�ᱣ�ֹ���
            stateMachine.ReusableData.MovementSpeedModifier = rollData.SpeedModifier;

            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.RollParemeterHash);

            //����֮�� ���ܳ��
            stateMachine.ReusableData.ShouldSprint = false;

            startTime = Time.time;
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.RollParemeterHash);
        }
        public override void Update()
        {
            base.Update();

            // ���ձ��ϣ����� AnimationEvent δ������1.0���ǿ��˳�����
            if (Time.time > startTime + 1.0f)
            {
                if (stateMachine.ReusableData.MovementInput == Vector2.zero)
                {
                    stateMachine.ChangeState(stateMachine.MediumStoppingState);
                }
                else
                {
                    OnMove();
                }
            }
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            //��������� �ͷ���  û���� ������ת
            //ȷ���ڲ�����Move��ʱ�� ������ת
            if(stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }
            RotateTowardsTargetRotation();
        }
        /// <summary>
        /// ���ö���ת���¼�
        /// </summary>
        public override void OnAnimationTransitionEvent()
        {
            //���������û���ƶ� ��ֹͣ���ƶ��� �ٿ���
            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.MediumStoppingState);

                return;
            }
            //����ƶ� ����Ѿ�false�� ���Բ��ø���move
            OnMove();
        }
        #endregion

        #region Input Methods
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
           //�����ǲ�ϣ������
        }
        #endregion
    }
}

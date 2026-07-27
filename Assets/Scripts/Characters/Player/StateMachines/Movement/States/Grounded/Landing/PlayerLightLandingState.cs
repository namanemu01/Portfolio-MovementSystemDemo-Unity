using UnityEngine;

namespace MovementStstem
{
    public class PlayerLightLandingState : PlayerLandingState
    {
        private float startTime;

        public PlayerLightLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        //2.����ʱ �߼�
        #region IState Methods
        public override void Enter()
        {

            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            base.Enter();

            //17 ����������½״̬�� ����ʹ����Ծ ����Ծ����Ϊ�̶���
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;

            ResetVelocity();

            startTime = Time.time;
        }

        //4.�л��ƶ�״̬ Ҫ�ڸ��·������ �����л����ƶ�״̬
        public override void Update()
        {
            //��Ϊͨ����Ծ ���������
            base.Update();

            // ���ձ��ϣ����� AnimationEvent δ������0.6���ǿ��˳�
            if (Time.time > startTime + 0.6f)
            {
                stateMachine.ChangeState(stateMachine.IdlingState);
                return;
            }

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

        //3.�л����ƶ���״̬ �ö��� �����¼� ���Դ���½ת������״̬
        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }
        #endregion
    }
}

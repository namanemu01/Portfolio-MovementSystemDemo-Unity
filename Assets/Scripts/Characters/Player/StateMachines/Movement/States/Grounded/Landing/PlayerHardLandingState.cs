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

        //1.����ʱ �߼�
        #region IState Methods
        //����֮��һ�� �����ƶ� ������������½֮ǰ���ٶ�
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.HardLandParemeterHash);
            //�������״̬ʱ �����ƶ� ֻ�ܵ�����ض�����������л�����һ��״̬
            //��Ե� �뿪״̬�����ƶ�
            stateMachine.Player.Input.PlayerActions.Movement.Disable();


            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.HardLandParemeterHash);

            //�뿪״̬ʱ �����ƶ���
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
            //��������ʱ Ҳ������½��������ʱ �����ƶ���
            stateMachine.Player.Input.PlayerActions.Movement.Enable();
        }

        //2.���� ÿ��״̬��д�л�����һ��״̬���߼� ���ù���һ��������״̬��ʲô��
        //ֻ��׹��״̬�Ż�������״̬ 
        //����Ҫ�˽�Ҫ���ʲô�����������л������״̬ �Լ������״̬�л���ʲô״̬
        //Ӳ��½���Գ�� ��������
        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }


        #endregion
        #region Reusable Methods
        //��Ϊ���������� ������Ҫ��д������� ������Ӳ��½״̬ʱ����ʹ������ ֻ��ʹ�ó�̺ͼ��ܵ� ��֡�����ƶ�

        //��ϰһ�»ص��Ǿ��������ʲô
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
            //��������������� ��ֱ�ӷ��� ��ִ��������л�״̬�߼�
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
            //硬着陆状态不能跳跃 这里留空意味着不执行跳跃
        }

        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
            //硬着陆状态不能冲刺
        }

        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            //�����ƶ� ���� ���
            OnMove();
        }
        #endregion
    }
}

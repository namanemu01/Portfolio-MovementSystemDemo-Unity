using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    //��ʮ����������ֹͣ״̬�е�һ�������� ������������״̬���̳�
    public class PlayerStoppingState : PlayerGroundedState
    {
        //Ҫ���� ֹͣ����֮�� �����������ֹͣ ��״̬

        // ���ﱣ��ֹ����¼�δ����ʱ��ʱ�˳�
        private float enterTime;
        protected virtual float AnimationEventFallbackTimeout => 1.5f;

        public PlayerStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        #region IState Methods
        override public void Enter()
        {
            //�������״̬��ʱ�� �Ȱ��ٶ�����Ϊ0
            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            SetBaseCameraRecenteringData();

            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.StoppingParemeterHash);

            enterTime = Time.time;

        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.StoppingParemeterHash);
        }
        public override void Update()
        {
            base.Update();

            // ���ﱣ�ϣ����� AnimationEvent û�д�������ʱ�˳�ֹͣ״̬
            if (Time.time > enterTime + AnimationEventFallbackTimeout)
            {
                stateMachine.ChangeState(stateMachine.IdlingState);
            }
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            //14��ת ��Ϊ֮ǰ���������ʱ���Ѿ�д���� ����ֱ���þ�����
            RotateTowardsTargetRotation();

            //�����״̬�� �ٶ�������С ���ͣ����
            //���м��ͼ���
            if(!IsMovingHorizontally())
            {
                //true ˵��û���ƶ�
                return;
            }
            DecelerateHorizontally();
        }
        //��д���������¼����� ������д��ֹͣ״̬���ɵ�����״̬���߼� ��Ϊ�����Ǵ�ֹͣ״̬�������״̬ ��������¼������ڶ�������д�� ��ֹͣ�������ɵ������������¼�
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
            stateMachine.Player.Input.PlayerActions.Movement.started -= OnMovementStarted;
        }
        #endregion

        #region Input Methods
        //�����״̬�� ȡ�����벻�������״̬ ��Ϊ����ֹͣ״̬ ȡ������Ӧ�ý���ֹͣ״̬ �����Ǵ���״̬
       
            //���ÿ�����Ϊֹ֮ͣ��������� �����ƶ�ȡ������ص����� 
            //��Ϊ�������ԭ�����ݾ����л��� ����״̬ ���ظ��ˣ�����ֱ�Ӱ������óɿվͺ���
        

        
        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            //����ת�Ƶ��ƶ�״̬
            OnMove();
        }
        #endregion
    }
}

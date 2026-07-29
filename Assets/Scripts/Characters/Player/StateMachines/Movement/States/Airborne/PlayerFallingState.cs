using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    public class PlayerFallingState : PlayerAirborneState
    {
        private PlayerFallData fallData;

        private Vector3 playerPositionOnEnter;
        public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            fallData = airborneData.FallData;
        }

        #region IState Methods �ӿ�״̬���� ��Ϊװ��������Ǽ̳нӿڷ�����
        public override void Enter()
        {
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.FallParemeterHash);

          //  Debug.Log(GetPlayerVerticalVelocity().y);
            playerPositionOnEnter = stateMachine.Player.transform.position;


            //��������״̬ʱ �������ƶ�
            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            //��������״̬ʱ ������ҵĴ�ֱ�ٶȣ�ȷ����ҷ�����������
            ResetVerticalVelocity();

            // [DEBUG] Log entry state for diagnosing fall-to-landing stuck bug用于诊断落地卡死Bug 的日志条目状态
            //Debug.Log($"[DEBUG-Fall] Enter | PosY={stateMachine.Player.transform.position.y:F3} | VelY={GetPlayerVerticalVelocity().y:F3} | Input={stateMachine.ReusableData.MovementInput}");
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.FallParemeterHash);

        }
        
        /// <summary>
        /// ���Ӵ�ֱ�� ��ֹ�ٶ��ر����ײ����͸���� �޷���⵽������ ��������������������״̬ʱ����һ���Ĵ�ֱ�ٶȣ�������Ϊ�������ٶȹ�������Ҵ�͸�����޷���⵽����¼���
        /// </summary>
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // [DEBUG] Visualize ground-check ray and log hit/miss for diagnosing stuck fall
            //可视化地面检测射线并记录命中/未命中日志，用于诊断下落卡死问题
            //DebugGroundCheckRay();

            LimitVerticalVelocity();
        }

        #endregion

        #region Reusable Methods �����÷���
        protected override void ResetSprintState()
        {
            //ȷ��������״̬ʱ ����ʹ�ó��
        }

        //17ʹӲ��½������½������������
        //*�����Ҫ����׹���˺� �������������������һ���¼� �����������״̬ʱ������������������˺�
        /// <summary>
        /// ��ȡ�������ľ��� �ӽ����ʱ�򱣴�λ�� �ٱ����������ײ��λ��
        /// </summary>
        /// <param name="collider"></param>
        protected override void OnContactWithGround(Collider collider)
        {
            // [DEBUG] Log ground contact details for diagnosing stuck fall
            //记录地面接触详情，用于诊断下落卡死问题
            float fallDistance = playerPositionOnEnter.y - stateMachine.Player.transform.position.y;
           // Debug.Log($"[DEBUG-Fall] OnContactWithGround | collider={collider.name} | fallDistance={fallDistance:F3} | MinHardFall={fallData.MinimumDistanceToBeConsideredHardFall:F3} | ShouldWalk={stateMachine.ReusableData.ShouldWalk} | ShouldSprint={stateMachine.ReusableData.ShouldSprint} | Input={stateMachine.ReusableData.MovementInput}");

            //����������С����С���� ���л�������½״̬
            if (fallDistance < fallData.MinimumDistanceToBeConsideredHardFall)
            {
                stateMachine.ChangeState(stateMachine.LightLandingState);

                return;
            }
            //���������������С���� ���Ҳ����� ���л���Ӳ��½״̬
            //��������� ���������ģʽ���Ҳ��ܳ�� ����û������ ���л���Ӳ��½״̬ ����������������½��Ӳ��½������
            if (stateMachine.ReusableData.ShouldWalk && !stateMachine.ReusableData.ShouldSprint || stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.HardLandingState);
                return;
            }
            //���������������С���� ���������� ���л�������״̬
            stateMachine.ChangeState(stateMachine.RollingState);
        }
        #endregion
        #region Main Methods ��Ҫ����
        private void LimitVerticalVelocity()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

            //�����ҵĴ�ֱ�ٶȳ����������ٶ����ƣ����������ڸ÷�Χ��
            //��Ϊ���� �����Ǹ��� if��Ҫ��true��ִ��
            if (playerVerticalVelocity.y>=-fallData.FallSpeedLimit)
            {
               //���С����������ٶ� ��ֹ�������������������� -6>-5false������
                return;
            }

            //���������������ٶ� ִ������ ����Ǽ�ⴹֱ�ٶȷ�ֹ��ģ����Ӱ���ָе�


            Vector3 limitedVelocity = new Vector3(0f,-fallData.FallSpeedLimit-playerVerticalVelocity.y,0f);

            stateMachine.Player.Rigidbody.AddForce(limitedVelocity, ForceMode.VelocityChange);
        }

        // [DEBUG] Helper method to draw ground-check ray and log results
       /* private void DebugGroundCheckRay()
        {
            //起点
            Vector3 rayOrigin = stateMachine.Player.transform.position + Vector3.up * 0.1f;
            Vector3 rayDirection = Vector3.down;//射线方向向下
            float rayLength = 2f;//长度两米

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                Debug.Log($"[DEBUG-Fall] GroundCheckRay HIT | object={hit.collider.name} | distance={hit.distance:F3} | point={hit.point}");
                Debug.DrawRay(rayOrigin, rayDirection * hit.distance, Color.green);
            }
            else
            {
                Debug.Log("[DEBUG-Fall] GroundCheckRay MISS");
                Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);
            }
        }*/
        #endregion
    }
}

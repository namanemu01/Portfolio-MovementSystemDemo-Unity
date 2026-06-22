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

        #region IState Methods 接口状态方法 因为装这个的类是继承接口方法的
        public override void Enter()
        {
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.FallParemeterHash);

            //记录进入下落状态时玩家的位置 用来计算下落距离
            playerPositionOnEnter = stateMachine.Player.transform.position;


            //进入下落状态时 不允许移动
            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            //进入下落状态时 重置玩家的垂直速度，确保玩家发生其他问题
            ResetVerticalVelocity();
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.FallParemeterHash);

        }
        public override void Update()
        {
            base.Update();
            if(GetPlayerVerticalVelocity().y > 0)
            {
                return;
            }
            stateMachine.ChangeState(stateMachine.FallingState);

        }
        /// <summary>
        /// 添加垂直力 防止速度特别高碰撞器穿透地面 无法检测到玩家落地 这个方法是让玩家在下落状态时保持一定的垂直速度，避免因为重力加速度过大导致玩家穿透地面无法检测到落地事件。
        /// </summary>
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            LimitVerticalVelocity();
        }

        #endregion

        #region Reusable Methods 可重用方法
        protected override void ResetSprintState()
        {
            //确保在下落状态时 不能使用冲刺
        }

        //17使硬着陆和轻着陆产生条件区分
        //*如果想要添加坠落伤害 可以在这个方法里添加一个事件 让玩家在下落状态时根据下落距离来计算伤害
        /// <summary>
        /// 获取玩家下落的距离 从进入的时候保存位置 再保存与地面碰撞的位置
        /// </summary>
        /// <param name="collider"></param>
        protected override void OnContactWithGround(Collider collider)
        {
            //计算下落距离 得到玩家在进入下落状态时的位置和当前玩家位置的y轴差值的绝对值
            float fallDistance = playerPositionOnEnter.y - stateMachine.Player.transform.position.y;

            //如果下落距离小于最小距离 就切换到轻着陆状态
            if (fallDistance < fallData.MinimumDistanceToBeConsideredHardFall)
            {
                stateMachine.ChangeState(stateMachine.LightLandingState);

                return;
            }
            //如果下落距离大于最小距离 并且不输入 就切换到硬着陆状态
            //这里的条件 如果是行走模式并且不能冲刺 或者没有输入 就切换到硬着陆状态 这样就区分了轻着陆和硬着陆的条件
            if (stateMachine.ReusableData.ShouldWalk && !stateMachine.ReusableData.ShouldSprint || stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.HardLandingState);
                return;
            }
            //如果下落距离大于最小距离 并且有输入 就切换到滚动状态
            stateMachine.ChangeState(stateMachine.RollingState);
        }
        #endregion
        #region Main Methods 主要方法
        private void LimitVerticalVelocity()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

            //如果玩家的垂直速度超过了下落速度限制，则将其限制在该范围内
            //因为下落 所以是负数 if需要是true才执行
            if (playerVerticalVelocity.y>=-fallData.FallSpeedLimit)
            {
               //如果小于最大下落速度 终止这个方法，这个方法结束 -6>-5false进不来
                return;
            }

            //如果大于最大下落速度 执行下面

            
            Vector3 limitedVelocity = new Vector3(0f,-fallData.FallSpeedLimit-playerVerticalVelocity.y,0f);
            
            stateMachine.Player.Rigidbody.AddForce(limitedVelocity, ForceMode.VelocityChange);
        }
        #endregion
    }
}

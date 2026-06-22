using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    public class PlayerJumpingState : PlayerAirborneState
    {
        //15.4
        private PlayerJumpData jumpData;
        //15.3
        //判断是否需要继续旋转的变量
        private bool shouldKeepRotating;
        //16.4 用来防止在跳跃下落时 玩家进入下落状态
        private bool canStartFalling;

        public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            jumpData = airborneData.JumpData;
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            stateMachine.ReusableData.RotationData = jumpData.RotationData;

            //15.6
            stateMachine.ReusableData.MovementDecelerationForce = jumpData.DecelerationForce;

            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            //如果玩家在跳跃时有输入，那么我们就继续旋转到输入方向，否则就不旋转
            shouldKeepRotating = stateMachine.ReusableData.MovementInput!=Vector2.zero;

            Jump();
        
        }


        // 退出时将玩家的旋转数据重置为基础旋转数据，以确保在下次进入跳跃状态时能够正确应用旋转数据
        public override void Exit()
        {
            base.Exit();
            SetBaseRotationData();

            //重置开关 恢复初始
            canStartFalling = false;
        }
        /// <summary>
        /// 从跳跃状态 过渡到 下落状态的条件是玩家的垂直速度小于或等于0，这意味着玩家已经达到跳跃的最高点，开始下落了。
        /// 通过检查是不是速度是负数 从而进入下降状态
        /// 如果用动画最高点下降时过渡 则可能在上方有障碍时不能正确过渡到下降状态 因为速度可能还没有变成负数
        /// </summary>
        public override void Update()
        {
            base.Update();

            //Debug.Log("canStartFalling = " + canStartFalling);
            //Debug.Log("垂直速度 = " + GetPlayerVerticalVelocity().y);

            //表示到达最高点 可以开始下落了
            if (!canStartFalling && IsMovingUp(0f))
            {
                //打开开关 允许进入下落状态
                canStartFalling = true;
            }

            //如果不能开始下落 或者 玩家正在上升 就不进入下落状态
            if (!canStartFalling || GetPlayerVerticalVelocity().y>0)
            {
                return;
            }

            //如果玩家的垂直速度小于或等于0，说明玩家已经达到跳跃的最高点，开始下落了，此时需要切换到下落状态
            stateMachine.ChangeState(stateMachine.FallingState);

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (shouldKeepRotating)
            {
                RotateTowardsTargetRotation();
            }

            //15.5防止漂浮
            if(IsMovingUp())
            {
                //垂直轴添加力
                DecelerateVertically();
            }
        }

        #endregion

        #region Reusable Methods
        protected override void ResetSprintState()
        {
            //跳跃状态不需要重置冲刺状态，所以覆盖父类方法并留空
            //这样除了跳跃状态以外的空降状态就会重置冲刺状态了
        }
        #endregion
        #region Main Methods
        private void Jump()
        {

            //在这里添加临时变量的原因是因为 跳跃力 需要根据 倾斜角度和跳跃方向进行更改
            //意思就是说 如果不修改的话，那个力就一直是那个方向
            //通过创建临时变量 我们可以根据需要更新他 而不用更改原始属性值
            Vector3 jumpforce = stateMachine.ReusableData.CurrentJumpForce;
              
            Vector3 jumpDirection = stateMachine.Player.transform.forward;

            if (shouldKeepRotating)
            {
                //19.3将目标旋转更新为相对于我们输入和相机
                UpdateTargetRotation(GetMovementInputDirection());
                //获取玩家输入的方向 作为跳跃的方向
                jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
            }

            jumpforce.x *= jumpDirection.x;
            jumpforce.z *= jumpDirection.z;

            //15.4判断斜坡 射线
            Vector3 capsuleCplliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;
            //创建
            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleCplliderCenterInWorldSpace,Vector3.down);
            //投射
            if(Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit,jumpData.JumpToGroundRayDistance,stateMachine.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
            {
               //如果用射线击中某物，需要知道是否在斜坡上
               float groundAngle = Vector3.Angle(hit.normal, downwardsRayFromCapsuleCenter.direction);

                //检查角度并根据需要调整跳跃力 使用动画曲线完成此操作
                //为每种情况都设置动画曲线
                if(IsMovingUp())
                { 
                    //只需要在水平轴上添加力
                    float forceModifier = jumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);
                    jumpforce.x *= forceModifier;
                    jumpforce.z *= forceModifier;
                }

                if(IsMovingDown())
                {
                    //只需要在垂直轴上添加力
                    float forceModifier = jumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);
                    jumpforce.y *= forceModifier;
                }
            }


            //在跳跃之前重置玩家的速度，以确保跳跃力的效果不会被当前速度所影响
            ResetVelocity();

            //此模式是因为与时间和质量无关
            stateMachine.Player.Rigidbody.AddForce(jumpforce, ForceMode.VelocityChange);
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            
        }
        #endregion
    }
}

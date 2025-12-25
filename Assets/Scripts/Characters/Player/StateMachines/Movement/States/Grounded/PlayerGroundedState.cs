using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    /// <summary>
    /// 11.1 将所有地面状态的共用逻辑提取到这个类里
    /// </summary>
    public class PlayerGroundedState : PlayerMovementState
    {
        //需要在斜率数据那类里拿到浮动射线距离 这样写就不需要很长一行了
        private SlopeData slopeData;

        public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            //猜测是为了共享实例 这样改了一个另一个也能改 或者节省资源 
            slopeData = stateMachine.Player.ColliderUtility.SlopeData;
        }

        #region IState Methods 接口状态方法 因为装这个的类是继承接口方法的
        /// <summary>
        /// 14.1 重写物理更新方法 是为了写浮动胶囊体 是只有地面状态才有的逻辑
        /// </summary>
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Float();
        }
        #endregion

        #region Main Methods 主要方法
        /// <summary>
        /// 浮动胶囊体 方法
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Float()
        {
            //获取胶囊体碰撞器在世界空间的位置
            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

            //计算胶囊体底部位置 从胶囊体中心位置发射向下射线
            Ray downWardsRayFromCapsuleCenter = new Ray(
                capsuleColliderCenterInWorldSpace,//起点
                Vector3.down//方向
                );
            //第二个参数是返回储存信息的结构体
            //第四个参数是需要发生碰撞的一个或多个层 建立单独的脚本来保存
            //第五个参数是查询触发器交互的枚举类型 忽略触发器 意思是射线不会与触发器碰撞 会忽略该层中碰撞器是触发器的物体
            if (Physics.Raycast(downWardsRayFromCapsuleCenter,out RaycastHit hit,slopeData.FloatRayDistance,stateMachine.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
            {
                //要处理上坡角度越大速度越慢的 逻辑 第一个参数是射线检测到物体的法线向量 第二个参数是玩家向下的方向向量的反方向
                //这样得到这个角的对角的余角刚好是地面和斜坡之间的夹角
                float groundAngle = Vector3.Angle(hit.normal,-downWardsRayFromCapsuleCenter.direction);

                //设置改变速度的方法
                float slopeSpeedModifier =  SetSlopeSpeedModifierOnAngle(groundAngle);
                //如果斜坡速度修改器是0 说明不能移动 这让我们不能在斜坡上浮动
                if (slopeSpeedModifier == 0) return;

                //计算胶囊体底部到地面（如台阶）的距离 缩放模型时为了确保浮动距离正确 需要乘以缩放值 不然胶囊体跟着变大了 浮动距离还是原来的就不对了
                //这个缩放值是本地缩放值 因为胶囊体碰撞器的尺寸是根据本地缩放计算的 
                //下一步是需要从射线命中中减去这个距离 保证本地和世界空间一致

                //是计算胶囊体底部到地面的距离 所以用中心点 减去 底部到地面的 距离
                float distanceToFloatPoint = stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderCenterInLoaclSpace.y*stateMachine.Player.transform.localScale.y-hit.distance;
                if (distanceToFloatPoint == 0f) return;

                //需要一个升力 变量名字叫需提升重力 下面是计算上抬力的算式
                float amountToLift = distanceToFloatPoint*slopeData.StepReachForce-GetPlayerVerticalVelocity().y;
                //然后需要这个值与额外力相乘 并删除当前的垂直速度↑
                //然后减去当前的垂直速度


                //总上抬力 = 弹簧力 - 阻尼力
                //        = (位移 × 弹簧系数) -当前速度

                //得到一个向上的力的向量 垂直力
                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
                //应用这个力
                stateMachine.Player.Rigidbody.AddForce(liftForce,ForceMode.VelocityChange);

                //这一步弄完之后 这个胶囊体就浮动了 没加的时候 会在没有胶囊体的地方掉下地面
                //模型脚部穿模了可以尝试ik来解决一下 这里没有使用
            }
        }

        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            //使用动画曲线来设置斜坡速度修改器 而非if语句
            //这使我们可以 更直观地轻松的调整 斜坡速度修改器 在地面数据脚本里，这个脚本是显示在窗口里的
            //14.2 此时检查窗口里面的动画曲线已经设置完成
            //获取给定时间的值
            float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

            //将值赋给重用数据里的斜坡速度修改器
            stateMachine.ResuableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

            return slopeSpeedModifier;
        }
        #endregion


        //回调是用来监听输入的，看玩家什么时候松开按键
        //这个类的操作是为了节省资源，因为走路和跑步状态都需要监听松开按键事件
        //所以把这个监听放在它们的父类里
        //利用多态还是可以让他们各自拥有自己的回调方法
        #region Reusable Methods 可重用方法
        /// <summary>
        /// 9.5给另一个状态添加一个回调 现在在基类里添加是为了节省回调监听资源
        /// </summary>
        protected override void AddInputActionsCallBacks()
        {
            //9.5添加移动和取消操作
            base.AddInputActionsCallBacks();

            stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;

            //15.6添加冲刺开始回调
            stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;

        }

    

        protected override void RemoveInputActionsCallBacks()
        {
            base.RemoveInputActionsCallBacks();

            stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;

            stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;

        }

        /// <summary>
        /// 11.2这一步是将按键控制走路和跑步变成公共shouldWalk变量控制，之前是每个状态有独立的控制
        /// </summary>
        protected virtual void OnMove()
        {
            if (stateMachine.ResuableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);
                return;
            }


            //当前如果不是应该走路状态 那应该是跑步状态
            stateMachine.ChangeState(stateMachine.RunningState);

            //我们现在可以从待机空闲状态切换到其他状态了 ^-^
            //下一步是添加其他状态里的 逻辑
        }
        #endregion

        #region 输入方法 Input Methods

        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            //松开按键切换到待机状态，让walk和run都继承这个方法，他们俩是一样的因为一松手就要切换到待机状态
            //不需要为了独立性重新walk和run各自写一个↑
            stateMachine.ChangeState(stateMachine.IdlingState);
        }

        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            //15.6 输入方法 冲刺开始时 切换到冲刺状态
            stateMachine.ChangeState(stateMachine.DashingState);
        }
        #endregion

    }
}

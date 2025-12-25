using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    //15.1 创建冲刺状态 并加入到运动状态状态机中储存
    public class PlayerDashingState : PlayerGroundedState
    {
        private PlayerDashData dashData;

        //15.6 限制冲刺问题
        private float startTime;
        private int ContinuousDashesUsed;
        //15.2两种情况：
        //移动时冲刺 朝向移动方向冲刺 速度调节器提高即可 （添加修改值 于是需要写一个冲刺数据脚本
        //静止时冲刺 朝向朝向方向冲刺 添加一个力 （因为没有移动输入 所以速度调节器无效
        public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            dashData = movementData.DashData;
        }
        #region IState Methods
        public override void Enter()
        {
            base.Enter();
            //15.4进入的时候 如果是移动状态 将速度修改为 经过速度调节器修改的 通过总类运动状态链接地面数据脚本
            stateMachine.ResuableData.MovementSpeedModifier = dashData.SpeedModifier;

            //15.5 待机状态的话
            AddForceOnTransitionFromStationaryState();

            //15.6
            UpdateConsecutiveDashes();

            //15.6记录开始时间
            startTime = Time.time;
        }
        /// <summary>
        /// 15.8复用基类的动画过渡事件方法 也就是现在先写方法 做动画机的时候里再添加事件
        /// </summary>
        public override void OnAnimationTransitionEvent()
        {
            base.OnAnimationTransitionEvent();
            if(stateMachine.ResuableData.MovementInput==Vector2.zero)
            {
                //如果是静止状态 进入硬停止状态（因为没有写硬停止状态 所以先进入待机状态
                stateMachine.ChangeState(stateMachine.IdlingState);
                return;
            }
 
             //否则进入疾跑状态
             stateMachine.ChangeState(stateMachine.SprintingState);
            
        }

        #endregion

        #region Main Methods
        private void AddForceOnTransitionFromStationaryState()
        {
            //区分移动还是静止状态 可以通过判断输入
            if(stateMachine.ResuableData.MovementInput!=Vector2.zero)
            {
                return;
            }

            //玩家面朝向 只需要水平方向
            Vector3 characterRotationDirection = stateMachine.Player.transform.forward;
            characterRotationDirection.y = 0f;

            stateMachine.Player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed();
        }

        /// <summary>
        /// 15.6 更新连续冲刺方法
        /// </summary>
        private void UpdateConsecutiveDashes()
        {
            //15.7检查是否是连续冲刺 （次数没达到 不需要等待时间/时间达到了 次数归零

            //if里面依旧是true才进入 所以需要里面的方法返回false 表示不是连续按冲刺
            if (!IsConsecutive())
            {
                //如果已经过了限制时间 重置可以连续冲刺的次数
                ContinuousDashesUsed = 0;
            }
            //如果返回true 可以冲刺 增加一次计数
            ++ContinuousDashesUsed;

            //检查已使用的冲刺计数是否等于冲刺限制数
            if(ContinuousDashesUsed== dashData.ConsecutiveDashesLimitAmount)
            {
                ContinuousDashesUsed = 0;
                //并且禁用输入几秒钟 于是单独写脚本
                stateMachine.Player.Input.DisableActionFor(stateMachine.Player.Input.PlayerActions.Dash,dashData.DashLimitReachedCooldown);
            }
        }

        private bool IsConsecutive()
        {
            //当前游戏时间 小于输入的上一个冲刺时间 加上 被认为是连续冲刺的时间
            //此处如果是true 说明当前时间设定小于连续时间 也就是不能再冲刺
            return Time.time < startTime + dashData.TimeToBeConsideredConsecutive;
        }
        #endregion

        #region Input Methods 输入回调
        //15.8 重写移动取消回调
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            //需要在冲刺状态下松开移动键 进入硬停止状态
            //所以这里重写基类的方法
            //因为原来这个方法是进入走路或跑步状态 松开按键就是待机状态 默认 现在需要在这个类里改写
        }
        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
            //这里要完成的逻辑是 持续冲刺 停下之后接得是硬停止状态
            //使用动画事件在关键帧 添加事件 并且可以在动画进入该帧时调用特定方法
            //但是需要创建动画之后才能测试 现在最多就是写方法
        }
        #endregion

        //目前冲刺状态就这些功能 之后需要动画机和动画来配合完成剩下的功能
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 层级状态机，作为一个状态机的基类状态接口使用
    /// </summary>
    public interface IState
    {
        public void Enter();//进入状态时调用,状态机接口
        public void Exit();//退出状态时调用
        public void HandleInput();//进行输入处理
        public void Update();//进行非物理更新
        public void PhysicsUpdate();//进行物理更新

        //15.7因为动画机添加方法
        public void OnAnimationEnterEvent();//当玩家进入动画第一帧使玩家免受伤害
        public void OnAnimationExitEvent();//当玩家离开动画最后一帧使玩家可以受伤
        public void OnAnimationTransitionEvent();//动画进入到某一帧 转换状态
    }
}

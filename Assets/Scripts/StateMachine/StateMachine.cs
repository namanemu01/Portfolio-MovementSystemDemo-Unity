using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 状态机抽象类 这里面处理接口的具体实现 类似于一个状态管理器 多态
    /// </summary>
    public abstract class StateMachine 
    {
        protected IState currentState;//当前状态
        /// <summary>
        /// 改变状态 这种用法类似于不等于空就调用方法 判空的简写 
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(IState newState)
        {
            currentState?.Exit();

            currentState = newState;

            currentState.Enter();
        }

        public void HandleInput()
        {
            currentState?.HandleInput();
        }

        public void Update()
        {
            currentState?.Update();
        }

        public void PhysicsUpdate()
        {
            currentState?.PhysicsUpdate();
        }
        
        public void OnAnimationEnterEvent()
        {
            currentState?.OnAnimationEnterEvent();
        }
        public void OnAnimationExitEvent()
        {
            currentState?.OnAnimationEnterEvent();
        }
        public void OnAnimationTransitionEvent()
        {
            currentState?.OnAnimationEnterEvent();
        }
    }
}

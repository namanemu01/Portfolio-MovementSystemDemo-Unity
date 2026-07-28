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
        protected IState currentState;//接口里当前状态
        /// <summary>
        /// 改变状态 
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
            currentState?.OnAnimationExitEvent();
        }
        public void OnAnimationTransitionEvent()
        {
            currentState?.OnAnimationTransitionEvent();
        }
        public void OnTriggerEnter(Collider collider)
        {
            currentState?.OnTriggerEnter(collider);
        }
        public void OnTriggerExit(Collider collider)
        {
            currentState?.OnTriggerExit(collider);
        }

        //15.2
        public void OnTriggerEnter(Collider collider)
        {
            //这个调用的是istate里面那个
            currentState?.OnTriggerEnter(collider);
        }
        public void OnTriggerExit(Collider collider)
        {
            //这个调用的是istate里面那个
            currentState?.OnTriggerExit(collider);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementStstem
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerInputActions InputActions { get; private set; }
        //下面这个是playermap，因为我叫player所以他会在后面加actions，就叫这个名字
        public PlayerInputActions.PlayerActions PlayerActions { get; private set; }

        private void Awake()
        {
            //实例化这个输入动作类
            InputActions = new PlayerInputActions();
            //获取player的输入动作映射，也就是上面的那个playermap，让他俩关联起来
            PlayerActions = InputActions.Player;
        }

        //启用输入动作,在onenable中启用,虽然有实例化但是不启用是没用的，所以需要启用
        //启用和禁用对应生命周期，比如在disable中禁用
        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

        /// <summary>
        /// 16.6 手写禁止输入几秒钟的方法 在这里写是因为协程只能在mono用
        /// </summary>
        public void DisableActionFor(InputAction action,float seconds)
        {
            //使用协程 比循环每次调用更好 更适合
            //协程的名字以及参数 
            StartCoroutine(DisableAction(action,seconds));
        }


        private IEnumerator DisableAction(InputAction action, float seconds)
        {
            //不让action行动
            action.Disable();
            //等待
            yield return new WaitForSeconds(seconds);
            //可以行动
            action.Enable();
        }
    }

}

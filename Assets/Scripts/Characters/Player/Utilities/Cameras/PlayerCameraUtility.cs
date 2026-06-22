using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 思路是先设置值 然后再使用方法 设置默认值
    /// </summary>
    [Serializable]
    public class PlayerCameraUtility 
    {
        //18获取对虚拟相机的引用
        [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
        //对默认值的引用 默认水平等待时间和默认水平居中时间
        [field: SerializeField] public float DefaultHorizontalWaitTime { get; private set; } = 0f;
        //
        [field: SerializeField] public float DefaultHorizontalRecenteringTime { get; private set; } = 4f;

        //获得对水平居中选项的参考
        private CinemachinePOV cinemachinePOV;

        /// <summary>
        /// 获得pov组件的引用
        /// </summary>
        public void Initialize()
        {
            cinemachinePOV = VirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        }
        /// <summary>
        /// 创建一个启用和禁用 重新居中 选项的方法
        /// </summary>
        public void EnableRecentering(float waitTime = -1f,float recenteringTime = -1f,float baseMovementSpeed = 1f,float movementSpeed = 1f)
        {
            cinemachinePOV.m_HorizontalRecentering.m_enabled = true;

            //18.2

            //取消相机现有的居中设置 这是自带的方法
            cinemachinePOV.m_HorizontalRecentering.CancelRecentering();

            if(waitTime == -1f)
            {
                waitTime = DefaultHorizontalWaitTime;
            }

            if (recenteringTime == -1f)
            {
                recenteringTime = DefaultHorizontalRecenteringTime;
            }

            //18.2使速度越快 居中时间越短
            recenteringTime = recenteringTime * baseMovementSpeed / movementSpeed;

            //设置水平居中 选项值
            cinemachinePOV.m_HorizontalRecentering.m_WaitTime = waitTime;
            cinemachinePOV.m_HorizontalRecentering.m_RecenteringTime = recenteringTime;

        }
        public void DisableRecentering()
        {
            cinemachinePOV.m_HorizontalRecentering.m_enabled = false;
        }
        //18然后把这个变成玩家的属性
    }
}

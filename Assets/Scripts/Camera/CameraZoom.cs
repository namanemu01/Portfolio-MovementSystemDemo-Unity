using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 这个类用于控制摄像机的缩放
    /// </summary>
    public class CameraZoom : MonoBehaviour
    {
        //摄像机缩放的三个等级：默认，混合，最大
        [SerializeField][Range(0f, 10f)] private float defaultDistance = 6f;
        [SerializeField][Range(0f, 10f)] private float minimumDistance = 1f;
        [SerializeField][Range(0f, 10f)] private float maximumDistance = 6f;

        //一个是用于平滑距离差值的值，另一个是与z轴相乘的值，也就是缩放灵敏度
        [SerializeField][Range(0f, 10f)] private float smoothing = 4f;
        [SerializeField][Range(0f, 10f)] private float zoomSensitivity = 1f;

        //下一步是引用摄像机组件上面相机距离和输入z轴的脚本得到变量
        //所以就得得到组件在awake里
        private CinemachineFramingTransposer farmingTransposer;
        private CinemachineInputProvider inputProvider;

        //设置一个修正值
        private float currentTargetDistace;

        private void Awake()
        {
            farmingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
            inputProvider = GetComponent<CinemachineInputProvider>();

            //让修正值一开始等于默认值，这是你想让目标离你多远，目的地
            currentTargetDistace = defaultDistance;
        }

        private void Update()
        {
            Zoom();
        }
        /// <summary>
        /// 这个方法实现缩放逻辑
        /// </summary>
        private void Zoom()
        {
            //这个值获得是输入的值乘灵敏度的值
            float zoomValue = inputProvider.GetAxisValue(2)*zoomSensitivity;
            //计算目标相机距离，基于当前输入变化，将距离值控制在最大和最小之间
            currentTargetDistace = Mathf.Clamp(zoomValue+ currentTargetDistace, minimumDistance, maximumDistance);
            //获取当前相机的实际距离
            float currentDistance = farmingTransposer.m_CameraDistance;

            //检查这俩距离是否需要更新，如果相等就不更新，返回，不相等则执行下面的逻辑
            if( currentDistance == currentTargetDistace)
            {
                return;
            }
            //计算平滑过渡值
            float lerpedZoomValue = Mathf.Lerp(currentDistance, currentTargetDistace,smoothing*Time.deltaTime);
            //赋值
            farmingTransposer.m_CameraDistance = lerpedZoomValue;
        }
    }
}

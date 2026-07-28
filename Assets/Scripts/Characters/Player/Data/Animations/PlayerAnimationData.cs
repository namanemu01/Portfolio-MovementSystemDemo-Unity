using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    [Serializable]
    public class PlayerAnimationData
    {
        //20 需要参数名称 一个用于储存hash的属性
        [Header("State Group Parameter Name")]
        //汉译 地面参数名称 确保字符串与动画控制器参数具有完全相同的名称
        [SerializeField] private string groundedParameterName = "Grounded";
        [SerializeField] private string movingParameterName = "Moving";
        [SerializeField] private string stoppingParameterName = "Stopping";
        [SerializeField] private string landingParameterName = "Landing";
        [SerializeField] private string airborneParameterName = "Airborne";

        [Header("Ground Parameter Name")]
        [SerializeField] private string idleParameterName = "isIdling";
        [SerializeField] private string dashParameterName = "isDashing";
        [SerializeField] private string walkParameterName = "isWalking";
        [SerializeField] private string runParameterName = "isRunning";
        [SerializeField] private string sprintParameterName = "isSprinting";
        [SerializeField] private string mediumStopParameterName = "isMediumStopping";
        [SerializeField] private string hardStopParameterName = "isHardStopping";
        [SerializeField] private string rollParameterName = "isRolling";
        [SerializeField] private string hardLandParameterName = "isHardLanding";

        [Header("Airborne Parameter Name")]
        [SerializeField] private string fallParameterName = "isFalling";

        //为hash创建属性 为上面的每个变量复制此行并重复命名他们
        public int GroundedParemeterHash { get; private set; }
        public int MovingParemeterHash { get; private set; }
        public int StoppingParemeterHash { get; private set; }
        public int LandingParemeterHash { get; private set; }
        public int AirborneParemeterHash { get; private set; }

        public int IdleParemeterHash { get; private set; }
        public int DashParemeterHash { get; private set; }
        public int WalkParemeterHash { get; private set; }
        public int RunParemeterHash { get; private set; }
        public int SprintParemeterHash { get; private set; }
        public int MediumStopParemeterHash { get; private set; }
        public int HardStopParemeterHash { get; private set; }
        public int RollParemeterHash { get; private set; }
        public int HardLandParemeterHash { get; private set; }

        public int FallParemeterHash { get; private set; }
        /// <summary>
        /// 初始化 使标签变成hash值
        /// </summary>
        public void Initialize()
        {
            GroundedParemeterHash = Animator.StringToHash(groundedParameterName);
            MovingParemeterHash = Animator.StringToHash(movingParameterName);
            StoppingParemeterHash = Animator.StringToHash(stoppingParameterName);
            LandingParemeterHash = Animator.StringToHash(landingParameterName);
            AirborneParemeterHash = Animator.StringToHash(airborneParameterName);

            IdleParemeterHash = Animator.StringToHash(idleParameterName);
            DashParemeterHash = Animator.StringToHash(dashParameterName);
            WalkParemeterHash = Animator.StringToHash(walkParameterName);
            RunParemeterHash = Animator.StringToHash(runParameterName);
            SprintParemeterHash = Animator.StringToHash(sprintParameterName);
            MediumStopParemeterHash = Animator.StringToHash(mediumStopParameterName);
            HardStopParemeterHash = Animator.StringToHash(hardStopParameterName);
            RollParemeterHash = Animator.StringToHash(rollParameterName);
            HardLandParemeterHash = Animator.StringToHash(hardLandParameterName);
            
            FallParemeterHash = Animator.StringToHash(fallParameterName);
           
        }
    }
}

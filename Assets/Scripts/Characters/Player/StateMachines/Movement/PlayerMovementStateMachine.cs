using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    public class PlayerMovementStateMachine : StateMachine
    {
        //在状态机中持有对玩家的引用，以便各个状态可以访问玩家的数据和方法
        //也就是说这个类里面装的是 提前缓存下来的一些状态 便于随时调用
        public Player Player { get; }

        public PlayerStateReuseableData ReusableData { get; }

        public PlayerIdlingState IdlingState { get; }
        public PlayerDashingState DashingState { get; }

        public PlayerWalkingState WalkingState { get; }
        public PlayerRunningState RunningState { get; }
        public PlayerSprintingState SprintingState { get; }

        //23stop后补的
        public PlayerLightStoppingState LightStoppingState { get; }
        public PlayerMediumStoppingState MediumStoppingState { get; }
        public PlayerHardStoppingState HardStoppingState { get; }

        //23land
        public PlayerLightLandingState LightLandingState { get; }
        public PlayerRollingState RollingState { get; }
        public PlayerHardLandingState HardLandingState { get; }


        //23
        public PlayerJumpingState JumpingState { get; }
        public PlayerFallingState FallingState { get; }
        public PlayerMovementStateMachine(Player player)
        {
            //将玩家引用传递给状态机
            Player = player;

            //12.3下一步使用数据变量交换值
            ReusableData = new PlayerStateReuseableData();

            //在构造函数中初始化所有状态
            //每个加入this，是需要每个继承自该状态的状态生成一个构造函数
            IdlingState = new PlayerIdlingState(this);
            DashingState = new PlayerDashingState(this);

            WalkingState = new PlayerWalkingState(this);
            RunningState = new PlayerRunningState(this);
            SprintingState = new PlayerSprintingState(this);

            LightStoppingState = new PlayerLightStoppingState(this);
            MediumStoppingState = new PlayerMediumStoppingState(this);
            HardStoppingState = new PlayerHardStoppingState(this);

            LightLandingState = new PlayerLightLandingState(this);
            RollingState = new PlayerRollingState(this);
            HardLandingState = new PlayerHardLandingState(this);

            JumpingState = new PlayerJumpingState(this);
            FallingState = new PlayerFallingState(this);
        }
    }
}

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

        public PlayerStateReuseableData ResuableData { get; }

        public PlayerIdlingState IdlingState { get; }
        public PlayerDashingState DashingState { get; }
        public PlayerWalkingState WalkingState { get; }
        public PlayerRunningState RunningState { get; }
        public PlayerSprintingState SprintingState { get; }

        public PlayerMovementStateMachine(Player player)
        {
            //将玩家引用传递给状态机
            Player = player;

            //12.3下一步使用数据变量交换值
            ResuableData = new PlayerStateReuseableData();

            //在构造函数中初始化所有状态
            //每个加入this，是需要每个继承自该状态的状态生成一个构造函数
            IdlingState = new PlayerIdlingState(this);
            DashingState = new PlayerDashingState(this);
            WalkingState = new PlayerWalkingState(this);
            RunningState = new PlayerRunningState(this);
            SprintingState = new PlayerSprintingState(this);

        }
    }
}

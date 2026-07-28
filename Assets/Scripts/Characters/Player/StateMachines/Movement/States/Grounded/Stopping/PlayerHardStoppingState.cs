

namespace MovementStstem
{
    public class PlayerHardStoppingState : PlayerStoppingState
    {
        public PlayerHardStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.HardStopParemeterHash);

            stateMachine.ReusableData.MovementDecelerationForce = movementData.StopData.HardDecelerationForce;

            //15
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.HardStopParemeterHash);
        }
        #endregion

        #region Reusable Methods
        protected override void OnMove()
        {
           if(stateMachine.ReusableData.ShouldWalk)
            {
                return;
            }

           stateMachine.ChangeState(stateMachine.RunningState);
        }
        #endregion
    }
}

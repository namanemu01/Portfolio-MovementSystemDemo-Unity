using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 动画事件转发器：挂在带 Animator 的 Y Bot 子物体上，
    /// 将 AnimationEvent 转发给父物体的 Player 状态机。
    /// </summary>
    public class AnimationEventForwarder : MonoBehaviour
    {
        [SerializeField] private Player player;

        // 这三个函数名必须和动画片段里 AnimationEvent 的函数名完全一致
        private void OnAnimationEnterEvent()
        {
            player?.OnAnimationEnterEvent();
        }

        private void OnAnimationExitEvent()
        {
            player?.OnAnimationExitEvent();
        }

        private void OnAnimationTransitionEvent()
        {
            player?.OnAnimationTransitionEvent();
        }
    }
}

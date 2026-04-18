using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [field:Header("References")]
        //12.3 设置参考在player脚本里 并参考类型为playerso
        [field:SerializeField] public PlayerSO Data {  get; private set; }

        [field: Header("Collisions")]
        //13.6将写好的胶囊体碰撞器工具类放在这里
        //16.2 这个工具类是为了让我们更方便地管理玩家的碰撞器，尤其是当玩家需要根据状态改变碰撞器尺寸时，这个工具类可以帮助我们计算和更新碰撞器的尺寸，避免了在多个状态类中重复编写相同的代码。
        [field:SerializeField]public PlayerCapsuleColliderUtility ColliderUtility { get; private set; }
        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }

        //18
        [field: Header("Cameras")]
        [field: SerializeField] public PlayerCameraUtility CameraUtility { get; private set; }


        //2.这节课写的是让玩家根据相机的方向转向相机，并且朝此方向移动，所以我们需要一个摄像机的transform变量
        public Transform MainCameraTransform { get; private set; }

        //20.2
        [field:Header("Animations")]
        [field:SerializeField]public PlayerAnimationData AnimationData { get; private set; }

        //1.我们需要一个刚体变量用来接收玩家的刚体组件,有了刚体我们才能用物理方法移动玩家
        public Rigidbody Rigidbody { get; private set; }
        //1.我们需要声明一个玩家输入类的变量，用来接收玩家的输入
        //1.因为这个这个脚本挂载玩家身上，然后用getcomponent获取玩家输入组件
        
        //对animator的引用
        public Animator Animator { get; private set; }

        public PlayerInput Input { get; private set; }

        private PlayerMovementStateMachine movementStateMachine;
        private void Awake()
        {

            Rigidbody = GetComponent<Rigidbody>();
            //首先在父级搜索 其次在子级搜索
            Animator = GetComponentInChildren<Animator>();

            //获取玩家输入组件
            Input = GetComponent<PlayerInput>();

            //调用初始化
            ColliderUtility.Initialize(gameObject);
            //确保碰撞器尺寸 更新
            ColliderUtility.CalculateCapsuleColliderDimensions();

            //18.awake是在实例化的时候调用
            CameraUtility.Initialize();

            //20
            AnimationData.Initialize();
            //获取主摄像机的transform，因为cinemachine虚拟摄像机是跟随主摄像机移动的
            //这是一个将它缓存起来的好方法，避免每次使用时都调用Camera.main，提升性能
            //面试：camera之前版本main的性能问题和解决方案
            //并且我们在玩家状态类里面加上玩家旋转方法
            MainCameraTransform = Camera.main.transform;

            //创建玩家状态机实例
            movementStateMachine = new PlayerMovementStateMachine(this);

        }
        /// <summary>
        /// 因为只有awake有的话只会一开始调用一次 
        /// 这个方法是使检查窗口数据更改时同时调用 目前窗口会弹出警告 之后再改
        /// </summary>
        private void OnValidate()
        {
            //调用初始化
            ColliderUtility.Initialize(gameObject);
            //确保碰撞器尺寸 更新
            ColliderUtility.CalculateCapsuleColliderDimensions();
        }
        private void Start()
        {
           
            //初始化状态机，让玩家进入游戏默认状态设置成待机状态
            movementStateMachine.ChangeState(movementStateMachine.IdlingState);

        }

        private void OnTriggerEnter(Collider collider)
        {
            movementStateMachine.OnTriggerEnter(collider);
        }

        private void OnTriggerExit(Collider collider)
        {
            movementStateMachine.OnTriggerExit(collider);
        }
        private void Update()
        {
            movementStateMachine.HandleInput();//先处理输入
            movementStateMachine.Update();//再进行非物理更新

        }
        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();//进行物理更新

        }
    }
}

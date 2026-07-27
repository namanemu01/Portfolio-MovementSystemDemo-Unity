using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [field:Header("References")]
        //12.3 ���òο���player�ű��� ���ο�����Ϊplayerso
        [field:SerializeField] public PlayerSO Data {  get; private set; }

        [field: Header("Collisions")]
        //13.6��д�õĽ�������ײ���������������
        [field:SerializeField]public PlayerCapsuleColliderUtility ColliderUtility { get; private set; }
        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }

        [field:Header("Cameras")]
        [field:SerializeField]public PlayerCameraUtility CameraUtility { get;private set; }

        [field:Header("Animations")]
        [field:SerializeField]public  PlayerAnimationData AnimationData { get; private set; }

        //2.��ڿ�д��������Ҹ�������ķ���ת����������ҳ��˷����ƶ�������������Ҫһ���������transform����
        public Transform MainCameraTransform { get; private set; }

        //1.������Ҫһ�������������������ҵĸ������,���˸������ǲ��������������ƶ����
        public Rigidbody Rigidbody { get; private set; }
        //1.������Ҫ����һ�����������ı���������������ҵ�����
        //1.��Ϊ�������ű�����������ϣ�Ȼ����getcomponent��ȡ����������
        
        public Animator Animator { get; private set; }
        public PlayerInput Input { get; private set; }
        
        private PlayerMovementStateMachine movementStateMachine;
        private void Awake()
        {

            Rigidbody = GetComponent<Rigidbody>();
            //获取子物体上的 Animator（Y Bot）
            Animator = GetComponentInChildren<Animator>();

            //获取输入脚本
            Input = GetComponent<PlayerInput>();

            //关键引用判空检查
            if (Data == null)
            {
                Debug.LogError("[Player] PlayerSO (Data) 未在 Inspector 中赋值！", this);
                return;
            }
            if (ColliderUtility == null)
            {
                Debug.LogError("[Player] ColliderUtility 未在 Inspector 中赋值！", this);
                return;
            }
            if (CameraUtility == null)
            {
                Debug.LogError("[Player] CameraUtility 未在 Inspector 中赋值！", this);
                return;
            }
            if (AnimationData == null)
            {
                Debug.LogError("[Player] AnimationData 未在 Inspector 中赋值！", this);
                return;
            }

            //碰撞体初始化
            ColliderUtility.Initialize(gameObject);
            //确定碰撞体尺寸
            ColliderUtility.CalculateCapsuleColliderDimensions();
            CameraUtility.Initialize();
            AnimationData.Initialize();
            //获取主相机的transform，因为cinemachine相机是这个相机的子物体，跟随移动
            //这样做的好处是不用每次使用时都调用Camera.main，性能更好
            //注意：camera之前版本main返回的是活跃相机
            //所以，这里获取主相机的transform用于角色朝向计算
            if (Camera.main == null)
            {
                Debug.LogError("[Player] 场景中没有 tag 为 MainCamera 的相机！", this);
                return;
            }
            MainCameraTransform = Camera.main.transform;

            //创建移动状态机实例
            movementStateMachine = new PlayerMovementStateMachine(this);

        }
        /// <summary>
        /// ��Ϊֻ��awake�еĻ�ֻ��һ��ʼ����һ�� 
        /// ���������ʹ��鴰�����ݸ���ʱͬʱ���� Ŀǰ���ڻᵯ������ ֮���ٸ�
        /// </summary>
        private void OnValidate()
        {
            //只在编辑器中初始化碰撞体
            if (ColliderUtility == null)
            {
                return;
            }

            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();
        }
        private void Start()
        {
           
            //��ʼ��״̬��������ҽ�����ϷĬ��״̬���óɴ���״̬
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
            movementStateMachine.HandleInput();//�ȴ�������
            movementStateMachine.Update();//�ٽ��з���������

        }
        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();//������������

        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace MovementStstem
{
 
    //我感觉这个应该是一个总调用的逻辑，例如每个状态被调用的时候都要走一遍这个文件里面的方法，之后复习的时候再看
    public class PlayerMovementState : IState
    {
        //状态机可以引用player之后，状态对状态机进行引用,因为究极目标是在state里面引用player？

        //↓10节当前结构
        //！！所有的关联在这引用呢，找半天
        //现在逻辑上》状态机引用玩家，状态引用状态机，状态通过状态机引用玩家？
        //玩家动作状态类继承接口，玩家动作状态类引用玩家动作状态机类，地面状态类继承玩家动作状态类
        //其他动作小类继承地面状态类，地面状态类用于回调那些缩减代码。玩家动作状态机继承状态机类，状态机类装的是方法，玩家动作状态机类装的是属性和实例化那些状态小类


        protected PlayerMovementStateMachine stateMachine;

        //将玩家移动输入存储在这里，方便各个状态使用

        //下面是用替换重命名的方式骗过 通过不同引用来添加点.，，是为了把这个名字一下子全部改变，省的一个个改，并取消声明这个新变量
        //protected Vector2 stateMachine.ResuableData.MovementInput;
        //12.3 将movementinput重命名为可重用数据↑并且修改了其他位置，并且将其他相同性质都改
        //也就是和playerstatereusabledata里面早写好的要替换掉的变量交换一下
        //stateMachine.ResuableData.

        //现在写两个速度变量，用来存储不同状态下的速度
        //12.3当前只留这一个
        //protected float baseSpeed = 5f;
        //protected float stateMachine.ResuableData.MovementSpeedModifier = 1f;//第二个变量用于不改变基础速度的前提下修改速度，比如跑步，冲刺等

        //3.旋转玩家
        //当前目标角度
        //protected Vector3 stateMachine.ResuableData.CurrentTargetRotation;

        //时间变量，用于平滑旋转玩家
        //protected Vector3 stateMachine.ResuableData.TimeToReachTargetRotation;

        //平滑旋转所需的变量速度
        //protected Vector3 stateMachine.ResuableData.DampedTargetRotationCurrentVelocity;

        //平滑旋转所经过的时间
        //protected Vector3 stateMachine.ResuableData.DampedTargetRotationPassedTime;

        //9.4我们需要知道处于什么变量 就设定一个例如开启了就切换跑步状态那种
        //protected bool stateMachine.ResuableData.ShouldWalk;

        //12.4
        protected PlayerGroundedData movementData;


        //15
        protected PlayerAirborneData airborneData;
        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;

            //高
            movementData = stateMachine.Player.Data.GroundedData;

            airborneData = stateMachine.Player.Data.AirborneData;

            SetBaseCameraRecenteringData();

            InitializeData();
        }

     

        private void InitializeData()
        {
            SetBaseRotationData();
        }


        #region 实现接口脚本里面的方法 IState Methods
        //移动逻辑将写在这个文件

        public virtual void Enter()
        {
            //每当进入这个状态的时候，打印当前状态名字
            Debug.Log("State:" + this.GetType().Name);

            //9.4添加输入回调
            AddInputActionsCallBacks();
        }

        
        public virtual void Exit()
        {
            //9.4删除输入回调，在退出逻辑执行的时候
            RemoveInputActionsCallBacks();
        }


        public virtual void HandleInput()
        {
            ReadMovementInput();

        }

        public virtual void PhysicsUpdate()
        {
            Move();
        }



        public virtual void Update()
        {

        }

        //15.8下面是动画机方法
        public virtual void OnAnimationEnterEvent()
        {
           
        }

        public virtual void OnAnimationExitEvent()
        {
           
        }

        //动画过渡事件
        public virtual void OnAnimationTransitionEvent()
        {
            
        }
        //15.2
        public virtual void OnTriggerEnter(Collider collider)
        {
            //检查碰撞对象是不是环境
            if(stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGround(collider);

                //确保没有其他内容被调用
                return;
            }
        }
        public void OnTriggerExit(Collider collider)
        {
            if(stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                //如果玩家离开地面了 就切换到空中状态
                OnContactWithGroundExited(collider);
                return;
            }
        }

     

        #endregion

        #region 主方法Main Methods

        /// <summary>
        /// 读取玩家的移动输入
        /// </summary>
        private void ReadMovementInput()
        {
            //读取玩家的移动输入↓
            //调用引用出来的状态机类-》里面的玩家脚本-》里面的输入脚本，那个组件的
            //map，当时说了被命名为playeractions的=》这个里面的输入的vector2的值。
            stateMachine.ReusableData.MovementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
        }

        /// <summary>
        /// 实际移动玩家的方法
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Move()
        {
            //空转和跳跃不被视为移动 所以直接返回 不做移动操作
            if (stateMachine.ReusableData.MovementInput == Vector2.zero|| stateMachine.ReusableData.MovementSpeedModifier==0f)
            {
                return;
            }
            //获取玩家的移动方向
            Vector3 movementDirection = GetMovementInputDirection();

            //4.6所以在获取移动的时候，调用旋转方法很合适
            float targetRotationYAngle = Rotate(movementDirection);
            //将上面获取到的角度转换为玩家移动的方向
            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);


            //获取玩家的移动速度
            float movementSpeed = GetMovementSpeed();

            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
            //施加力移动玩家
            //注意，addforce是给一个已经存在的力再加上一个力，意味着如果，不停止按键，玩家会越来越快
            //而velocitychange是直接改变速度
            //为了解决这个问题，我们可以在每次移动前将玩家的速度重置为0，移除现有的速度
            stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed- currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
        }


        //第8节：让玩家根据相机方向移动和旋转 这个方法返回角度
        private float Rotate(Vector3 direction)
        {
            float directionAngle = UpdateTargetRotation(direction);


            //3.平滑旋转玩家
            RotateTowardsTargetRotation();

            return directionAngle;
            //4.5最后是添加相机旋转的调用，我们发现只有在走路的时候才跟着相机走，待机和跳跃没事
            //所以在运动方法里面调用就可以了 确实很耦合 可能亮点就是如何解状态机耦合
        }

        /// <summary>
        /// 将经过时间清零的方法 更新目标旋转数据 重置计时器
        /// </summary>
        /// <param name="directionAngle"></param>
        private void UpdateTargetRotationData(float targetAngle)
        {
            stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;
            stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0;
        }


        /// <summary>
        /// 添加输入方向角度
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private  float GetDirectionAngle(Vector3 direction)
        {
            //教程认为输入的角色移动的方向加上相机转动的度数会得到一个新的位移方向
            //所以首先我们要获取运动输入和相机的方向
            //这个是相机的方向：这个函数与unity的不同，角度是针旋转不同，tan2里面第一个参数是y，第二个是x。
            //然后将他们调转发现出来的角度与unity的角度相同了，原因是unity与本来的y=x对称的
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            //旋转需要将tan2的负角度改成正角度,
            //在Unity游戏开发中，角度必须控制在0-360°，原因就是负一百八会彻底破坏方向判断！
            //而数学上是正负一百八，因此我们要把数学上tan2给转换回来
            //目前获得了输入方向角
            //"把玩家按键方向 + 相机旋转角度 = 真正的移动方向"
            if (directionAngle < 0f)
            {
                directionAngle += 360f;
            }

            return directionAngle;
        }

        /// <summary>
        /// 添加相机角度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private float AddCameraRotationToAngle(float angle)
        {
            angle += stateMachine.Player.MainCameraTransform.eulerAngles.y;
            //用欧拉角是因为它是数值，四元数是无法直接设置的，而且相机y轴旋转是欧拉角，是水平旋转
            //判断是否有大于360度的情况
            if (angle > 360f)
            {
                angle -= 360f;
            }

            return angle;
        }

        #endregion
        #region reuseable methods可重用方法
        /// <summary>
        /// 20 避免经常使用getbool，直接写一个方法 参数是因为是一个值
        /// </summary>
        /// <param name="animationHash"></param>
        protected void StartAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, true);
        }
        protected void StopAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, false);
        }
        protected void SetBaseCameraRecenteringData()
        {
            //18.2
            stateMachine.ReusableData.BackwardsCameraRecenteringData = movementData.BackwardsCameraRecenteringData;
            stateMachine.ReusableData.SidewaysCameraRecenteringData = movementData.SidewaysCameraRecenteringData;
        }
        /// <summary>
        /// 添加回调
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void AddInputActionsCallBacks()
        {
            //后面这个加等于自定义的方法，也就是表示 按按钮的一瞬间加上这个方法
            //相当于一个按钮有三个委托，按下start，长按p，松开cancel，可以通过监听三个事件添加不同逻辑
            stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;

            //18还有两种情况分别是输入移动发生变化 鼠标移动时 可以调用
            stateMachine.Player.Input.PlayerActions.Look.started += OnMouseMovementStarted;
            stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;

            //18
            stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;
        }

        /// <summary>
        /// 删除回调
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void RemoveInputActionsCallBacks()
        {
            //删除回调
            stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;

            stateMachine.Player.Input.PlayerActions.Look.started -= OnMouseMovementStarted;
            stateMachine.Player.Input.PlayerActions.Movement.performed -= OnMovementPerformed;

            stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;
        }


        /// <summary>
        /// 14为了保证退出冲刺状态能重新设置旋转数据 变成了新方法 否则旋转继续是0.02秒 这个方法是为了在每个状态里都能调用，来设置基础旋转数据的
        /// </summary>
        protected void SetBaseRotationData()
        {
            //将基础旋转数据存储在可重用数据中，以便在多个状态之间共享和修改
            stateMachine.ReusableData.RotationData = movementData.BaseRotationData;

            //这个是发生轮换所耗费的时间变量
            stateMachine.ReusableData.TimeToReachTargetRotation = stateMachine.ReusableData.RotationData.TargetRotationReachTime;
        }

        /// <summary>
        /// 获取玩家的移动输入方向
        /// </summary>
        /// <returns></returns>
        protected Vector3 GetMovementInputDirection()
        {
            //将3d转换为2d，y轴为0，表示垂直方向不会移动
            return new Vector3(stateMachine.ReusableData.MovementInput.x, 0f, stateMachine.ReusableData.MovementInput.y);
        }

        /// <summary>
        /// 获取玩家的移动速度 包含了基础速度 修改器速度比如各种受伤状态 可重用修改器的速度也就是上坡速度 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected float GetMovementSpeed(bool shouldConsiderSlopes = true)
        {
            //19.2修改
            float movementSpeed = movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier;
            if (shouldConsiderSlopes)
            {
                movementSpeed *= stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
            }

            return movementSpeed;
        }

        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;
            playerHorizontalVelocity.y = 0f;
            return playerHorizontalVelocity;
        }

        /// <summary>
        /// 得到玩家垂直速度
        /// </summary>
        /// <returns></returns>
        protected Vector3 GetPlayerVerticalVelocity()
        {
            return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y,0f);
        }

        /// <summary>
        /// 旋转至目标朝向，这个方法汉译
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected void RotateTowardsTargetRotation()
        {
            //3.这个完全是根据方法然后找参数，方法是mathf.smoothdamp，使用平滑阻尼来旋转玩家
            //和第二个一样，先得到当前y角度，再得到输入目标角度，然后使用mathf.smoothdamp方法

            //得到当前y角度
            float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;
            //若是当前角度等于目标角度则不旋转
            if (currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y) return;
            //若是不等于目标角度，则使用平滑阻尼旋转玩家
            float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y,
                ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y,//ref是因为这个变量会被方法改变，unity自动传引用，ref是要在外面初始化
                //因为如果单独传入时间变量，那么每次调用这个方法时间都会被重置，达不到平滑效果，
                //每次调用平滑方法都是0.14f，而不是旋转花费
                stateMachine.ReusableData.TimeToReachTargetRotation.y- stateMachine.ReusableData.DampedTargetRotationPassedTime.y//目标所需旋转时间=总时间-已经经过的时间
                //Mathf.Infinity,
                //Time.fixedDeltaTime
                );

            //为上面的经过时间变量增加时间，不然每次调用都是0
            //因为这个方法是在fixedupdate调用的，所以用deltatime会返回fixedeltaTime
            stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            //现在有了平滑后的角度，将其应用到玩家刚体上
            //用四元数接收欧拉角
            Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);
            //应用到刚体上
            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);

            //另外，当平滑旋转完成后，我们需要重置经过时间变量dampedTargetRotationPassedTime，因为下次旋转又是从0开始的，而那个变量会一直增加


      
        }

        /// <summary>
        /// 更新目标旋转方法
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected float UpdateTargetRotation(Vector3 direction,bool shouldConsiderCameraRotation=true)
        {
            //1.添加输入方向角度
            float directionAngle = GetDirectionAngle(direction);

            //5.稍后用这个实现冲刺状态 dashing state
            if (shouldConsiderCameraRotation)
            {//下一步是获得相机角度,添加到输入角度上
            //也就是说，希望如果输入一个角度的时候，永远朝着相机所在的方向移动
            //比如说不加的时候，相机怎么转，物体都是朝着世界坐标系的那个方向移动，加了摄像机度数之后会变成在相机朝向基础上移动
            //2.添加相机角度
            directionAngle = AddCameraRotationToAngle(directionAngle);

            }
            

            //4.如果相机角度不等于当前目标角度，执行时间重置方法
            if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }

            return directionAngle;
        }

        /// <summary>
        /// 这是一个把角度转方向的方法
        /// </summary>
        /// <param name="targetAngle"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected Vector3 GetTargetRotationDirection(float targetAngle)
        {
            //这是一个算数 角度转方向 应该有旋转矩阵的算法 输入度数 就能得到旋转之后的方向**
            //也就是说 四元数其实是一个复数,一个向量乘以一个复数,表示这个向量按照这个复数表示的角度旋转一个角度值
            //z轴是因为这是玩家向前的轴
            //矩阵乘法不满足交换律 不可以换位
            return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        /// <summary>
        /// 9.2使速度重置 瞬时清零
        /// </summary>
        protected void ResetVelocity()
        {
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        /// <summary>
        /// 重置垂直速度ResetVerticalVelocity
        /// </summary>
        protected void ResetVerticalVelocity()
        {
            //这个方法就会返回y=0的速度
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
            
            stateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
           
        }

        /// <summary>
        /// 13写减速方法，因为停止和后面没写的游泳都要用，所以写在状态机上层一点
        /// </summary>
        protected void DecelerateHorizontally()
        {
            //只需要在水平面上减速，所以y轴速度不变，因为有重力在拉着玩家
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
            //施加一个与当前水平速度相反的力来减速玩家,后面这个模式是想要一个不依赖玩家质量的减速效果，依赖时间，这个mode会乘以delta·time
            //想要设置玩家减速速度，需要乘以水平速度，会在每个状态中单独设置其他值，也就是PlayerStateResableData脚本
            stateMachine.Player.Rigidbody.AddForce(-playerHorizontalVelocity*stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
            //操作完成之后 需要创建必要的数据属性 以便能够通过检查器设置此力
        }
        /// <summary>
        /// 减速垂直力
        /// </summary>
        protected void DecelerateVertically()
        {
            
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
          
            stateMachine.Player.Rigidbody.AddForce(-playerVerticalVelocity * stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
           
        }

        ///13添加一个检查玩家是否在水平面上移动的方法，参数是一个最小速度的阈值，默认为0.1f
        protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
        {
            //这个参数很有必要的 因为要检查玩家水平速度大小来了解玩家是否正在移动
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
            //写这个是为了判断玩家水平速度的大小是否大于这个最小值，来判断玩家是否正在移动，因为有时候玩家可能会有一些微小的水平速度，这个时候我们不想把他当成在移动，所以设置一个最小值来过滤掉这些微小的速度
            Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);
            return playerHorizontalMovement.magnitude > minimumMagnitude;
        }

        /// <summary>
        /// 如果是在斜坡上 负责控制角度和力的大小 使用曲线斜率
        /// 这里是判断
        /// </summary>
        /// <param name="minimumVelocity"></param>
        /// <returns></returns>
        protected bool IsMovingUp(float minimumVelocity = 0.1f)
        {
            return GetPlayerVerticalVelocity().y > minimumVelocity;
        }

        /// <summary>
        /// 同上面的方法，负责检查负值
        /// </summary>
        /// <param name="minimumVelocity"></param>
        /// <returns></returns>
        protected bool IsMovingDown(float minimumVelocity = 0.1f)
        {
            return GetPlayerVerticalVelocity().y < -minimumVelocity;
        }

        /// <summary>
        /// 当玩家和地面接触时 需要发生什么就覆盖这个方法 重新加逻辑
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnContactWithGround(Collider collider)
        {

        }
        protected virtual void OnContactWithGroundExited(Collider collider)
        {

        }
        //18创建一个方法 更新居中值
        protected void UpdateCameraRecenteringState(Vector2 movementInput)
        {
            //没有移动就返回
            if(movementInput == Vector2.zero)
            {
                return;
            }
            if(movementInput == Vector2.up)
            {
                DisableCameraRecentering();

                return;
            }
            //得到相机垂直角度
            float cameraVerticalAngle = stateMachine.Player.MainCameraTransform.eulerAngles.x;
            //需要保持小角度而不要270那种
            if(cameraVerticalAngle >= 270f)
            {
                cameraVerticalAngle -= 360f;
            }
            //只检查正角度
            cameraVerticalAngle = Mathf.Abs(cameraVerticalAngle);
            //向后移动 查看是否在范围内 如果不在就禁用
            if(movementInput == Vector2.down)
            {
                SetCameraRecenteringState(cameraVerticalAngle, stateMachine.ReusableData.BackwardsCameraRecenteringData);

                return;
            }

            SetCameraRecenteringState(cameraVerticalAngle, stateMachine.ReusableData.SidewaysCameraRecenteringData);
        }
        /// <summary>
        /// 实现上面的方法 使用list 分别对应两个值
        /// </summary>
        /// <param name="cameraVerticalAngle"></param>
        /// <param name="cameraRecenteringData"></param>
        protected void SetCameraRecenteringState(float cameraVerticalAngle, List<PlayerCameraRecenteringData> cameraRecenteringData)
        {
            //
            foreach (PlayerCameraRecenteringData recenteringData in cameraRecenteringData)
            {
                if (!recenteringData.IsWithinRange(cameraVerticalAngle))
                {
                    continue;
                }
                EnableCameraRecentering(recenteringData.WaitTime, recenteringData.RecenteringTime);
                //找到所需的角度范围 退出方法
                return;

            }

            //如果没有找到任何于此角度相关的设置
            DisableCameraRecentering();
        }

        protected void EnableCameraRecentering(float waitTime = -1,float recenteringTime = -1f)
        {
            //18.2
            float movementSpeed = GetMovementSpeed();
            if(movementSpeed == 0f)
            {
                //这个设置避免除以零 的情况
                movementSpeed = movementData.BaseSpeed;
            }

            stateMachine.Player.CameraUtility.EnableRectentering(waitTime,recenteringTime,movementData.BaseSpeed,movementSpeed);
        }
        protected void DisableCameraRecentering()
        {
            stateMachine.Player.CameraUtility.DisableRecentering();
        }

        #endregion

        #region 输入方法 Input Methods

        //这个里面参数unity会自己解决，不用传
        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            //这里面的逻辑是 每次按下toggle的时候 都会把shouldwalk变成相反的值
            stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
            //我们每次进入一个状态就会添加一个回调 这意味着如果我们进入相同的回调，十次，所有我们也应该将他删除
            //完成步行切换回调
        }

        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            DisableCameraRecentering();
        }

        //这两个区别在于 start只在第一次按键的时候调用 performed是在每次案件的时候调用
        private void OnMouseMovementStarted(InputAction.CallbackContext context)
        {
            UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
        }
        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            //好像是要更新之后的值 这部分知识点也蛮复杂
            UpdateCameraRecenteringState(context.ReadValue<Vector2>());
        }


       



        #endregion
    }
}

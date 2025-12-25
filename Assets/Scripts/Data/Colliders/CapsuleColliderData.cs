using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 13.1装的是当前对象胶囊体的中间坐标
    /// </summary>
    public class CapsuleColliderData 
    {
        //13.1玩家挂载的那个胶囊体碰撞器
        public CapsuleCollider Collider {  get; private set; }
        //胶囊体碰撞器中心的本地储存
        public Vector3 ColliderCenterInLoaclSpace { get; private set; }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(GameObject gameObject)
        {
            //设置这个参数是因为根据游戏对象来设置，而不是通过检查器传入引用//用检查器也可以,如果你想每次从检查器传入的话，建议序列化此类

            //如果有内容就返回，没有内容就获得这个脚本物体
            if (Collider != null) return;
            Collider = gameObject.GetComponent<CapsuleCollider>();
            UpdateColliderData();
        }

        /// <summary>
        /// 一行代码也要提取方法 逻辑清晰化 *将中心缓存在本地空间中
        /// </summary>
        public void UpdateColliderData()
        {

            //获得中心的方法1世界坐标转局部坐标2胶囊体碰撞器自带,这个中心也就是浮动胶囊体的核心
            ColliderCenterInLoaclSpace = Collider.center;
        }
    }
}

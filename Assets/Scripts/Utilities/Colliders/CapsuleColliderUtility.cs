using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    /// <summary>
    /// 13胶囊体碰撞器工具 用来做浮动胶囊体 用来上楼梯 此类主要用途是寻找改变之后的中心坐标
    /// </summary>
    [Serializable]//需要在窗口显示滑块
    public class CapsuleColliderUtility 
    {
        //另一种方法是法线，查询为什么不用以及其他方法用什么上楼梯
        //我们需要的东西有 胶囊体碰撞器的引用 默认碰撞器数据 斜率数据，例如百分比滑块
        //数据和逻辑是分开的 所以需要创建数据data脚本

        //13.5为每个数据类创建一个变量
        [field:SerializeField] public CapsuleColliderData CapsuleColliderData {  get; private set; }
        [field: SerializeField] public DefaultColliderData DefaultColliderData { get; private set; }
        [field: SerializeField] public SlopeData SlopeData { get; private set; }

        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="gameObject"></param>
        public void Initialize(GameObject gameObject)
        {
            //运行时动态创建的方法需要实例化 提前准备好的方法不用实例化
            if (CapsuleColliderData != null) return;
            CapsuleColliderData = new CapsuleColliderData();
            CapsuleColliderData.Initialize(gameObject);
        }

        /// <summary>
        /// 13.5每次更新检查器时都会调用这个方法 计算胶囊体碰撞器尺寸 方法 为了得到中心坐标
        /// </summary>
        public void CalculateCapsuleColliderDimensions()
        {
            //已经设置好的半径数据把原本的半径数据更新
            SetCapsuleColliderRadius(DefaultColliderData.Radius);
            //需要高度乘以步高百分比,这里用1来删除表示的是，默认抬起百分之七十五
            SetCapsileColliderHeight(DefaultColliderData.Height * (1f - SlopeData.StepHeightPersentage));
            //这里浮动胶囊原理是一直保持最上面高度不变，挪动的是下面的位置，所以中心点会在高度差值一半的位置
            SetCapsileColliderCenter();

            //13.6写到此处会出现一个问题 当高度缩短到一定程度，胶囊体会呈现类似负数，因为最短是球体然后导致整个球体上移
            //所以写一个并不完美的方法解决这个问题 因为每次上升到大约半径二倍的时候才会出现这种情况，所有我们在此时缩短半径
            float halfColliderHeight = CapsuleColliderData.Collider.height / 2f;
            if (halfColliderHeight < CapsuleColliderData.Collider.radius)
            {
                //如果高度的一半小于半径，就让半径变成高度的一半 缩小半径
                SetCapsuleColliderRadius(halfColliderHeight);
            }

            //最终是为了得到胶囊体中心，并缓存
            //*再次将中心缓存在本地空间中
            CapsuleColliderData.UpdateColliderData();
        }


        public void SetCapsuleColliderRadius(float radius)
        {
            //玩家的胶囊体数据更新成这个新的半径
            CapsuleColliderData.Collider.radius = radius;
        }

        public void SetCapsileColliderHeight(float height)
        {
            //玩家的胶囊体数据更新成这个新的高度
            CapsuleColliderData.Collider.height = height;
        }

        public void SetCapsileColliderCenter()
        {
            float capsuleHeightDifference = DefaultColliderData.Height - CapsuleColliderData.Collider.height;
            Vector3 newColliderCenter = new Vector3(0f, DefaultColliderData.CenterY + (capsuleHeightDifference / 2f), 0f);
            //玩家的胶囊体数据更新成这个新的中心
            CapsuleColliderData.Collider.center = newColliderCenter;
        }

    }
}

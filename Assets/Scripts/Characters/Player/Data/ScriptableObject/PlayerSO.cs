using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementStstem
{
    //此类就是创建so文件的模板，产生的文件可以在编辑器中配置数据
    //必须放在继承自 ScriptableObject 的类上方。order菜单项在列表中的显示顺序。数字越小，排序越靠前,不写也行
    [CreateAssetMenu(fileName = "Player", menuName = "Custom/Characters/Player", order = 0)]
    /// <summary>
    /// so用来保存我们在系统中使用的大部分值，如速度修改器，旋转到达时间
    /// 继承的这个scriptableobject是unity自带的一个类，可以创建资产文件，也就是我们常说的so文件
    /// </summary>
    public class PlayerSO : ScriptableObject
    {
        //12.1做有关数据类这个往下的所有安排
        //界面可配，对外只读，如果不写序列化的话就不能在编辑器界面显示，这是针对属性写的序列化
        [field:SerializeField]public PlayerGroundedData GroundedData { get; private set; }
    }
}

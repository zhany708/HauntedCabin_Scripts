using System;
using UnityEngine;


/*
 * Introduction：用于敌人/事件生成前，提醒玩家的物体
 * Creator：Zhang Yu
*/

public class SpawnWarning : MonoBehaviour
{
    public event Action OnAnimationFinished;       //接收方为EnvironmentManager









    #region Unity内部函数
    private void Awake()
    {
        
    }
    #endregion


    #region 动画帧事件
    private void OnAnimationEnd()           //放在动画的最后一帧调用
    {
        OnAnimationFinished?.Invoke();          //调用回调函数
        Destroy(gameObject);
    }
    #endregion
}
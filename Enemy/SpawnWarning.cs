using System;
using UnityEngine;


/*
 * Introduction：用于敌人/事件生成前，提醒玩家的物体
 * Creator：Zhang Yu
*/

public class SpawnWarning : MonoBehaviour
{
    public event Action OnAnimationFinished;       //接收方为EnvironmentManager



    DoorController m_DoorController;

    GameObject m_EnemyObject;
    Animator m_Animator;                            //动画器






    #region Unity内部函数
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        if (m_Animator == null)
        {
            Debug.LogError("Cannot get the Aniamtor component in the: " + gameObject.name);
            return;
        }
    }
    #endregion


    #region 动画帧事件
    private void OnAnimationEnd()           //用于动画的最后一帧调用
    {
        OnAnimationFinished?.Invoke();          //调用回调函数
        Destroy(gameObject);
    }
    #endregion


    /*
    #region 主要函数
    public void GenerateEnemy()
    {
        //这里的enemy物体是敌人的跟物体（包含巡逻坐标的），在生成的同时赋予物体生成坐标
        GameObject enemyObject = EnemyPool.Instance.GetObject(m_EnemyObject, transform.position);     //从敌人对象池中生成敌人

        EnvironmentManager.Instance.InitializeEnemy(enemyObject, m_DoorController);     //初始化生成后的敌人
    }
    #endregion


    #region Setters
    public void SetEnemyObject(GameObject thisObject)
    {
        m_EnemyObject = thisObject;
    }

    public void SetDoorController(DoorController thisDoorController)
    {
        m_DoorController = thisDoorController;
    }
    #endregion
    */
}

using UnityEngine;


public class CoreComponent : MonoBehaviour
{
    protected Core core;

    protected Movement Movement
    {
        get
        {
            if (m_Movement) { return m_Movement; }      //检查组件是否为空
            m_Movement = core.GetCoreComponent<Movement>();
            return m_Movement;
        }
    }
    private Movement m_Movement;


    protected Combat combat
    {
        get
        {
            if (m_Combat) { return m_Combat; }      //检查组件是否为空
            m_Combat = core.GetCoreComponent<Combat>();
            return m_Combat;
        }
    }
    private Combat m_Combat;


    protected Stats Stats
    {
        get
        {
            if (m_Stats) { return m_Stats; }      //检查组件是否为空
            m_Stats = core.GetCoreComponent<Stats>();
            return m_Stats;
        }
    }
    private Stats m_Stats;


    protected ParticleManager particleManager => m_ParticleManager ? m_ParticleManager : core.GetCoreComponent(ref m_ParticleManager);      //问号表示如果问号左边变量为空，则返还冒号右边的函数，否则返还冒号左边的变量

    private ParticleManager m_ParticleManager;





    protected virtual void Awake()
    {
        
        core = transform.parent.GetComponent<Core>();       //从父物体那里调用Core组件

        if (!core)
        {
            Debug.LogError("There is no Core on the parent");
        }

        if (!Movement || !particleManager || !Stats)
        {
            Debug.Log("Something is wrong in the CoreComponent!");
        }

        core.Addcomponent(this);    //将所有需要运用LogicUpdate函数的组件加进List     
    }



    public virtual void LogicUpdate() {  }  
}
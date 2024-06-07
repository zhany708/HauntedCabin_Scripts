using UnityEngine;


public class CoreComponent : MonoBehaviour
{
    protected Core core;

    protected Movement movement
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


    protected Stats stats
    {
        get
        {
            if (m_Stats) { return m_Stats; }      //检查组件是否为空
            m_Stats = core.GetCoreComponent<Stats>();
            return m_Stats;
        }
    }
    private Stats m_Stats;






    protected virtual void Awake()
    {
        
        core = transform.parent.GetComponent<Core>();       //从父物体那里调用Core组件

        if (!core)
        {
            Debug.LogError("There is no Core component on the parent");
        }

        if (!combat || !stats)
        {
            Debug.Log("Some component is not assigned correctly in the CoreComponent!");
        }

        core.Addcomponent(this);    //将所有需要运用LogicUpdate函数的组件加进List     
    }



    public virtual void LogicUpdate() {  }  
}
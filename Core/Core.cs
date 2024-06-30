using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class Core : MonoBehaviour
{
    public Animator Animator { get; private set; }
    public AnimatorStateInfo AnimatorInfo { get; private set; }

    public float MaxHealth { get; private set; }
    public float Defense { get; private set; }
    public float HitResistance { get; private set; }     //击退抗性




    //将所有Core组件加进去。readonly用于保护List，防止运行时不小心重新赋值
    private readonly List<CoreComponent> m_CoreComponents = new List<CoreComponent>();      





    #region Unity内部函数
    private void Awake()
    {       
        //Debug.Log("Core Awake");
        Animator = GetComponentInParent<Animator>();        //调用父物体的动画控制器组件
        if (Animator == null)
        {
            Debug.LogError("Cannot get the Animator component in the: " + gameObject.parent.name);
            return;
        }   
    }

    public void LogicUpdate()       //由于该函数会在其余脚本里的Update函数中运行，因此放在“Unity内部函数”region内
    {       
        foreach (CoreComponent component in m_CoreComponents)
        {
            component.LogicUpdate();    //运行每个组建的LogicUpdate函数
        }
        
        AnimatorInfo = Animator.GetCurrentAnimatorStateInfo(0);
    }
    #endregion
 

    #region 获取子物体里的组件相关
    public T GetCoreComponent<T>() where T : CoreComponent          //T代表这是Generic函数
    {
        var comp = m_CoreComponents.OfType<T>().FirstOrDefault();   //返回第一个找到的值，否则返回基础值（大部分变量类型的基础值为null）

        if (comp) { return comp; }
        comp = GetComponentInChildren<T>();                         //如果找不到，则到子物体中寻找

        if (comp) { return comp; }
        
        Debug.LogWarning($"{typeof(T)} not found on {transform.parent.name}");      //如果仍然找不到，则报错
        return null;
    }

    public T GetCoreComponent<T>(ref T value) where T : CoreComponent
    {
        value = GetCoreComponent<T>();
        return value;       //返还组件的参考
    }
    #endregion


    #region 其余函数
    public void Addcomponent(CoreComponent component)
    {
        if (!m_CoreComponents.Contains(component))
        {
            m_CoreComponents.Add(component);     //如果组件不在List，则加进去
        }
    }
    #endregion


    #region Setters
    public void SetParameters(float maxHealth, float defense, float hitResistamce)  //设置参数，以供CoreComponent中的组件使用
    {
        MaxHealth = maxHealth;
        Defense = defense;
        HitResistance = hitResistamce;
    }
    #endregion
}

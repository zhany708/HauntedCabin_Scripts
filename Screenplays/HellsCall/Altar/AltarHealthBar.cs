using UnityEngine;




public class AltarHealthBar : HealthBar      //用于祷告石的血条控制
{
    Altar m_Altar;







    private void Awake() 
    {
        //从父物体那获取Altar组件
        m_Altar = GetComponentInParent<Altar>();
        if (m_Altar == null)
        {
            Debug.LogError("Altar component not found in the " + name);
            return;
        }
    }

    protected override void Start()
    {
        //游戏开始时初始化最大生命值
        SetMaxHp(m_Altar.GetMaxHealth());

        base.Start();
    }
}
using UnityEngine;


public class Combat : CoreComponent, Idamageable, IKnockbackable    //用于管理受击
{
    [SerializeField] private GameObject m_DamageParticles;

    public bool IsHit {  get; private set; }



    float m_HitResistance;     //击退抗性





    private void Start()
    {
        m_HitResistance = core.HitResistance;   //从Core那里获得参数
    }

    public void Damage(float amount)
    {
        IsHit = true;

        //Debug.Log(core.transform.parent.name + " Damaged!");
        Stats.DecreaseHealth(amount);

        particleManager.StartParticleWithRandomRotation(m_DamageParticles);   //造成伤害时在受击物体周围生成特效
    }

    /*
    public int GetHit(Vector2 direction)
    {
        return Movement.GetFlipNum(direction, Vector2.zero);

        //Debug.Log(core.transform.parent.name + " Faced you!");
    }
    */


    public void KnockBack(float strength, Vector2 direction)
    {
        if (strength > m_HitResistance)
        {
            //Debug.Log("You got knocked!");
            Movement.SetVelocity(strength - m_HitResistance, direction);      //只有当击退力度大于击退抗性时才会被击退
        }
    }

    #region Setters
    public void SetIsHit(bool isTrue)
    {
        IsHit = isTrue;
    }
    #endregion
}

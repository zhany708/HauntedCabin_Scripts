using UnityEngine;



public class Combat : CoreComponent, Idamageable, IKnockbackable    //用于管理受击
{
    [SerializeField] private GameObject m_DamageParticles;

    

    public bool IsHit {  get; private set; }

    /*
    //用于受击时的精灵图变白效果的Shader
    public Material FlashMaterial;

    SpriteRenderer m_SpriteRenderer;
    Material m_OriginalMaterial;        //精灵图的原始材质
    bool m_IsFlashing = false;      //表示精灵图是否在闪烁
    */

    float m_HitResistance;     //击退抗性
    






    /*
    protected override void Awake()
    {
        base.Awake();

        m_SpriteRenderer = GetComponentInParent<SpriteRenderer>();      //获取父物体的精灵组件
        m_OriginalMaterial = m_SpriteRenderer.material;
    }
    */

    private void Start()
    {
        m_HitResistance = core.HitResistance;   //从Core那里获得参数
    }

    public void Damage(float amount, bool doesIgnoreDefense)
    {
        IsHit = true;


        //Debug.Log(core.transform.parent.name + " Damaged!");
        Stats.DecreaseHealth(amount, doesIgnoreDefense);

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

    /*
    //通过Shader实现受击闪烁的效果（目前先不用）
    public void Flash()
    {
        if (m_IsFlashing) return; 

        StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        m_IsFlashing = true;
        m_SpriteRenderer.material = FlashMaterial;


        yield return new WaitForSeconds(0.1f);    //等待x秒

        //恢复材质
        m_SpriteRenderer.material = m_OriginalMaterial;
        m_IsFlashing = false;
    }
    */

    #region Setters
    public void SetIsHit(bool isTrue)
    {
        IsHit = isTrue;
    }
    #endregion
}

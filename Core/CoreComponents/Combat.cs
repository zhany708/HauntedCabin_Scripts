using UnityEngine;



public class Combat : CoreComponent, Idamageable, IKnockbackable    //用于管理受击
{
    //强行让受击粒子在编辑器中显示（可编辑）
    [SerializeField] private GameObject m_DamageParticle;

    

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


    #region Unity内部函数
    private void Start()
    {
        m_HitResistance = core.HitResistance;   //从Core那里获得参数
    }
    #endregion


    #region 伤害/受伤相关
    public void Damage(float amount, bool doesIgnoreDefense)        //受到伤害
    {
        IsHit = true;


        //Debug.Log(core.transform.parent.name + " Damaged!");
        stats.DecreaseHealth(amount, doesIgnoreDefense);

        //如果物体赋值了粒子特效，则造成伤害时在受击物体周围生成特效
        if (m_DamageParticle != null)
        {
            ParticleManager.Instance.StartParticleWithRandomRotation(m_DamageParticle);
        }    
    }
    #endregion
    
    
    /*
    public int GetHit(Vector2 direction)        //受击后转向攻击方
    {
        return Movement.GetFlipNum(direction, Vector2.zero);

        //Debug.Log(core.transform.parent.name + " Faced you!");
    }
    */


    #region 击退相关
    public void KnockBack(float strength, Vector2 direction)        //被击退
    {
        //只有当攻击物体的击退力度大于受击物体的击退抗性时，受击物体才会被击退
        if (strength > m_HitResistance)
        {
            //Debug.Log("You got knocked!");
            movement.SetVelocity(strength - m_HitResistance, direction);
        }
    }
    #endregion


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

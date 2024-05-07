using System.Collections;
using UnityEngine;



public class Combat : CoreComponent, Idamageable, IKnockbackable    //���ڹ����ܻ�
{
    [SerializeField] private GameObject m_DamageParticles;

    

    public bool IsHit {  get; private set; }

    /*
    //�����ܻ�ʱ�ľ���ͼ���Ч���Ĳ���
    public Material FlashMaterial;

    SpriteRenderer m_SpriteRenderer;
    Material m_OriginalMaterial;        //����ͼ��ԭʼ����
    bool m_IsFlashing = false;      //��ʾ����ͼ�Ƿ�����˸
    */

    float m_HitResistance;     //���˿���
    






    /*
    protected override void Awake()
    {
        base.Awake();

        m_SpriteRenderer = GetComponentInParent<SpriteRenderer>();      //��ȡ������ľ������
        m_OriginalMaterial = m_SpriteRenderer.material;
    }
    */

    private void Start()
    {
        m_HitResistance = core.HitResistance;   //��Core�����ò���
    }

    public void Damage(float amount)
    {
        IsHit = true;


        //Debug.Log(core.transform.parent.name + " Damaged!");
        Stats.DecreaseHealth(amount);

        particleManager.StartParticleWithRandomRotation(m_DamageParticles);   //����˺�ʱ���ܻ�������Χ������Ч
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
            Movement.SetVelocity(strength - m_HitResistance, direction);      //ֻ�е��������ȴ��ڻ��˿���ʱ�Żᱻ����
        }
    }

    /*
    //ͨ��Shaderʵ���ܻ���˸��Ч����Ŀǰ�Ȳ��ã�
    public void Flash()
    {
        if (m_IsFlashing) return; 

        StartCoroutine(DoFlash());
    }


    private IEnumerator DoFlash()
    {
        m_IsFlashing = true;
        m_SpriteRenderer.material = FlashMaterial;


        yield return new WaitForSeconds(0.1f);    //�ȴ�x��

        //�ָ�����
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

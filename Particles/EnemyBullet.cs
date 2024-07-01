using UnityEngine;



public class EnemyBullet : PlayerBullet
{
    public int DamageAmount;
    public float DamageKnockbackStrength;



    Vector2 m_AttackDirection;





    


    #region Unity内部函数
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Idamageable damageable = other.GetComponent<Idamageable>();
        IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();

        if (damageable != null)
        {
            damageable.Damage(DamageAmount, false);     //造成伤害，会受防御影响
            //damageable.GetHit(m_AttackDirection);
        }

        if (knockbackable != null)
        {
            knockbackable.KnockBack(DamageKnockbackStrength, m_AttackDirection);
        }

        ParticlePool.Instance.PushObject(gameObject);
    }
    #endregion


    #region 子弹相关
    public override void SetSpeed(Vector2 direction)
    {
        base.SetSpeed(direction);

        m_AttackDirection = direction;
    }
    #endregion
}

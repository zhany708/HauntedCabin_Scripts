using UnityEngine;



public class EnemyBullet : PlayerBullet
{
    public int DamageAmount;
    public float DamageKnockbackStrength;



    Vector2 m_AttackDirection;





    public override void SetSpeed(Vector2 direction)
    {
        base.SetSpeed(direction);

        m_AttackDirection = direction;
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Idamageable damageable = other.GetComponent<Idamageable>();
        IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();

        if (damageable != null)
        {
            damageable.Damage(DamageAmount, false);     //…À∫¶ ‹∑¿”˘”∞œÏ
            //damageable.GetHit(m_AttackDirection);
        }

        if (knockbackable != null)
        {
            knockbackable.KnockBack(DamageKnockbackStrength, m_AttackDirection);
        }

        ParticlePool.Instance.PushObject(gameObject);
    }
}

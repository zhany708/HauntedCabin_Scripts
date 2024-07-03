using UnityEngine;



public class EnemyBullet : PlayerBullet
{
    Enemy m_Enemy;                              //敌人的索引

    Vector2 m_AttackDirection;





    


    #region Unity内部函数
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Idamageable damageable = other.GetComponent<Idamageable>();
        IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();

        if (damageable != null)
        {
            damageable.Damage(m_Enemy.EnemyData.DamageAmount, false);     //造成伤害，会受防御影响
            //damageable.GetHit(m_AttackDirection);
        }

        if (knockbackable != null)
        {
            knockbackable.KnockBack(m_Enemy.EnemyData.KnockbackStrength, m_AttackDirection);
        }

        PushBulletBackToPool();
    }
    #endregion


    #region 子弹相关
    public override void SetSpeed(Vector2 direction)
    {
        //根据Data里的速度让子弹移动
        rigidBody2D.velocity = direction.normalized * m_Enemy.EnemyData.AttackSpeed;

        m_AttackDirection = direction;
    }
    #endregion


    #region Setters
    public void SetEnemy(Enemy thisEnemy)
    {
        m_Enemy = thisEnemy;
    }
    #endregion
}

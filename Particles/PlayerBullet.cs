using UnityEngine;


public class PlayerBullet : MonoBehaviour
{
    protected Rigidbody2D rigidBody2D;



    Coroutine m_DestroyBulletCoroutine;     //用于在子弹发射出去几秒后强制销毁子弹

    GunWeapon m_Gun;                        //玩家使用的枪械

    float m_ExistDuration = 4f;             //表示子弹最长可以存在的时间




    #region Unity内部函数
    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        //延迟几秒后强行销毁子弹
        m_DestroyBulletCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(m_ExistDuration, () =>
        {
            //Debug.Log("Time up for the bullet!");
            PushBulletBackToPool();
        }));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("The Player bullet collided with : " + other.gameObject.name);

        PushBulletBackToPool();       //使子弹碰到其他碰撞体（墙壁，家具等）时自毁
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("The Player bullet triggered the : " + other.gameObject.name);

        Idamageable damageable = other.GetComponent<Idamageable>();

        if (damageable != null)
        {
            //对被击中的敌人造成伤害（伤害受力量和敌人防御影响）
            damageable.Damage(m_Gun.GunData.AttackDetail.DamageAmount * PlayerStatusBar.Instance.GetStrengthAddition(), false);
            //damageable.GetHit(m_AttackDirection);
        }

        PushBulletBackToPool();
    }

    private void OnDisable()
    {
        //重置协程
        if (m_DestroyBulletCoroutine != null)
        {
            m_DestroyBulletCoroutine = null;
        }
    }
    #endregion


    #region 子弹相关
    public virtual void SetSpeed(Vector2 direction)
    {
        //根据子弹Data里的速度让子弹移动
        rigidBody2D.velocity = direction.normalized * m_Gun.GunData.AttackDetail.BulletSpeed;
    }


    protected void PushBulletBackToPool()
    {
        ParticlePool.Instance.PushObject(gameObject);
    }
    #endregion


    #region Setters
    public void SetWeapon(GunWeapon thisWeapon)
    {
        m_Gun = thisWeapon;
    }
    #endregion
}
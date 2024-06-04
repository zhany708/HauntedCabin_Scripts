using UnityEngine;


public class PlayerBullet : MonoBehaviour
{
    public float Speed;


    Rigidbody2D rigidBody2D;

    GunWeapon m_Gun;

    float m_ExistTimer;
    float m_CurrentTime;




    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        m_CurrentTime += Time.deltaTime;

        if (m_CurrentTime - m_ExistTimer >= 4)      //子弹生成4秒后强制销毁子弹
        {
            DestroyBullet();
        }
    }

    private void OnEnable()
    {
        m_ExistTimer = Time.time;
        m_CurrentTime = Time.time;
    }



    private void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("The Player bullet collided with : " + other.gameObject.name);

        DestroyBullet();       //使子弹碰到其他碰撞体（墙壁，家具等）时自毁
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("The Player bullet triggered the : " + other.gameObject.name);

        Idamageable damageable = other.GetComponent<Idamageable>();

        if (damageable != null)
        {
            //对被击中的敌人造成伤害（伤害受力量和敌人防御影响）
            damageable.Damage(m_Gun.GunData.AttackDetail.DamageAmount * PlayerStatusBar.GetStrengthAddition(), false);
            //damageable.GetHit(m_AttackDirection);
        }

        DestroyBullet();
    }





    public virtual void SetSpeed(Vector2 direction)
    {
        rigidBody2D.velocity = direction.normalized * Speed;
    }

    public void SetWeapon(GunWeapon thisWeapon)
    {
        m_Gun = thisWeapon;
    }




    private void DestroyBullet()
    {
        ParticlePool.Instance.PushObject(gameObject);
    }
}
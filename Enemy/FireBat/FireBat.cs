using UnityEngine;


public class FireBat : Enemy
{
    [SerializeField] GameObject m_FireBallPrefab;           //火球预制件





    #region Unity内部函数
    protected override void Start()
    {
        base.Start();

        AttackState = new FireBatAttackState(this, StateMachine, EnemyData, "Attack");    //将普通攻击状态改成火蝙蝠攻击状态
    }
    #endregion


    #region 特殊攻击相关
    public void FireBallLaunch(Transform target)
    {
        if (target != null)
        {
            //储存参数的临时坐标，防止函数运行期间参数消失
            Vector3 tempPos = target.position;

            //根据敌人的Transfom里的LocalScale判断敌人朝向
            Vector2 attackX = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;      

            //偏离参数（偏离嘴部多少）
            float deviation = 0.2f;     

            //火球生成位置在y轴上应位于头部，x轴上应偏离敌人的中心（嘴部发射）
            Vector2 attackPosition = Movement.Rigidbody2d.position + Vector2.up * 0.8f + attackX * deviation;       

            //计算火球与目标中心之间的夹角
            float angle = Mathf.Atan2((tempPos.y + 0.5f - attackPosition.y), (tempPos.x - attackPosition.x)) * Mathf.Rad2Deg;      

            //生成火球，并设置坐标和旋转
            GameObject FireBallObject = ParticlePool.Instance.GetObject(m_FireBallPrefab);
            FireBallObject.transform.position = attackPosition;
            FireBallObject.transform.rotation = Quaternion.Euler(0, 0, angle);



            EnemyBullet fireBall = FireBallObject.GetComponent<EnemyBullet>();                      //调用火球脚本

            fireBall.SetEnemy(this);                                                                //将本脚本的引用传给火球脚本
            fireBall.SetSpeed(tempPos + Vector3.up * 0.5f - FireBallObject.transform.position);     //朝目标中心方向发射火球
        }       
    }
    #endregion
}
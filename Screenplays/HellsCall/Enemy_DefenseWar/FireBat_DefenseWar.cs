using UnityEngine;


public class FireBat_DefenseWar : Enemy_DefenseWar
{
    public GameObject FireBallPrefab;


    protected override void Start()
    {
        base.Start();

        AttackState = new FireBatAttackState_DefenseWar(this, StateMachine, enemyData, "Attack");    //将普通攻击状态改成火蝙蝠攻击状态
    }




    public void FireBallLaunch(Transform target)
    {
        if (target != null)
        {
            //储存参数的临时坐标，防止函数运行期间参数消失
            Vector3 tempPos = target.position;


            Vector2 attackX = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;      //根据动画参数MoveX判断敌人朝向
            float deviation = 0.2f;     //偏离参数（偏离嘴部多少）
            Vector2 attackPosition = Movement.Rigidbody2d.position + Vector2.up * 0.8f + attackX * deviation;       //火球生成位置在y轴上应位于头部，x轴上应偏离敌人的位置（嘴部发射）


            float angle = Mathf.Atan2((tempPos.y + 0.5f - attackPosition.y), (tempPos.x - attackPosition.x)) * Mathf.Rad2Deg;      //计算火球与目标中心之间的夹角

            //生成火球，并设置坐标和旋转
            GameObject FireBallObject = ParticlePool.Instance.GetObject(FireBallPrefab);
            FireBallObject.transform.position = attackPosition;
            FireBallObject.transform.rotation = Quaternion.Euler(0, 0, angle);



            EnemyBullet fireBall = FireBallObject.GetComponent<EnemyBullet>();        //调用火球脚本
            fireBall.SetSpeed(tempPos + Vector3.up * 0.5f - FireBallObject.transform.position);        //朝目标中心方向发射火球
        }       
    }
}
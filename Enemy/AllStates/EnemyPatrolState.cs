using UnityEngine;


public class EnemyPatrolState : EnemyState
{
    //获取家具图层
    LayerMask m_FurnitureLayerMask = LayerMask.GetMask("Furniture");


    Vector2 m_RandomPosition;       //随机生成的坐标
    float m_PatrolTimer;    //用于巡逻的计时器


    //运用Physics2D检查重复坐标时需要的X和Y的值（火蝙蝠Y轴上有0.5的偏差，因为坐标点位于脚底）
    const float m_PhysicsCheckingXPos = 2f;
    const float m_PhysicsCheckingYPos = 4f;



    public EnemyPatrolState(Enemy enemy, EnemyStateMachine stateMachine, SO_EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }



    public override void Enter()
    {
        base.Enter();

        //生成随机坐标
        m_RandomPosition = enemy.PatrolRandomPos.GenerateSingleRandomPos();

        //生成完后，确保坐标没有碰撞到家具
        EnsurePositionIsValid();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        m_PatrolTimer += Time.deltaTime;

        enemy.EnemyFlip.FlipX(enemyMovement.GetFlipNum(m_RandomPosition, enemy.transform.position) );  //朝向巡逻点的方向

        if (enemy.Parameter.Target != null)     // && !enemy.CheckOutside())
        {
            stateMachine.ChangeState(enemy.ChaseState);     //巡逻时如果检测到玩家则切换为追击状态
        }

        //当距离目标巡逻点足够近时；或者如果5秒后敌人仍没有到达巡逻点（卡住了），则转换成闲置状态
        else if (Vector2.Distance(enemy.transform.position, m_RandomPosition) <= 0.1f || m_PatrolTimer >= 5f)
        {
            stateMachine.ChangeState(enemy.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (m_RandomPosition != null)
        {
            //移动到目标点
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, m_RandomPosition, enemyData.MoveSpeed * Time.deltaTime);
        }
    }

    public override void Exit()
    {
        base.Exit();

        m_PatrolTimer = 0f;     //退出时将计时器清零
    }







    //确保坐标处没有碰撞到家具
    private void EnsurePositionIsValid()
    {
        //用于防止无限循环的变量，最多循环100次
        int attemptCount = 0;

        while (!IsPositionEmpty(m_RandomPosition) && attemptCount < 100)
        {
            m_RandomPosition = enemy.PatrolRandomPos.GenerateSingleRandomPos();
            attemptCount++;
        }

        //超过最大循环数量后依然没有生成出合适的坐标时，则报错
        if (attemptCount >= 100)
        {
            Debug.LogError("Failed to find a valid patrol position after 100 attempts.");
        }
    }


    //运用物理函数检查要生成的坐标是否有家具
    private bool IsPositionEmpty(Vector2 position)
    {
        //第一个参数为中心点，第二个参数为长方形大小（沿着中心各延申一半），第三个参数为角度，第四个参数为检测的目标层级
        Collider2D overlapCheck = Physics2D.OverlapBox(position, new Vector2(m_PhysicsCheckingXPos, m_PhysicsCheckingYPos), 0f, m_FurnitureLayerMask);
        return overlapCheck == null;
    }
}
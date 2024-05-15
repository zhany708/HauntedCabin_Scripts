using System.Collections.Generic;
using UnityEngine;
using ZhangYu.Utilities;



public class DoorController : MonoBehaviour
{
    public Animator[] DoorAnimators;        //当前房间内所有门的动画器
    public GameObject[] EnemyObjects;       //当前房间内所有会生成的敌人
    public LayerMask FurnitureLayerMask;    //家具的Layer

    public Vector2 EnemySpawnPosNegativeOffset = Vector2.zero;     //敌人生成的负坐标范围（最左边和最下边的范围，x和y都是负数）
    public Vector2 EnemySpawnPosPositiveOffset = Vector2.zero;     //敌人生成的正坐标范围（最右边和最上边的范围，x和y都是正数）

    public Collider2D RoomTrigger {  get; private set; }


    public int KilledEnemyCount { get; private set; } = 0;          //表示当前房间内击杀了多少敌人
    public bool HasGeneratedEvent { get; private set; } = false;    //表示当前房间是否生成过事件
    public bool HasDeactivateEvent { get; private set; } = false;   //表示当前房间是否销毁了生成的事件




    RootRoomController m_MainRoom; 
    RandomPosition m_EnemySpwanPos;


    //运用Physics2D检查重复坐标时需要的X和Y的值（火蝙蝠Y轴上有0.5的偏差，因为坐标点位于脚底）
    const float m_PhysicsCheckingXPos = 2f;
    const float m_PhysicsCheckingYPos = 4f;


    bool m_IsRootRoom = false;              //表示当前门所在的房间是否为初始房间
    bool m_HasGeneratedEnemy = false;       //表示当前房间是否生成过敌人








    private void Awake()
    {
        RoomTrigger = GetComponent<Collider2D>();

        m_MainRoom = GetComponentInParent<RootRoomController>();

        if (EnemyObjects.Length != 0)   //如果房间有怪物
        {
            //敌人生成的x范围为房间坐标的x加变量中的值；生成的y范围为房间坐标的y加变量中的值
            Vector2 leftDownPos = new Vector2(m_MainRoom.transform.position.x + EnemySpawnPosNegativeOffset.x, m_MainRoom.transform.position.y + EnemySpawnPosNegativeOffset.y);
            Vector2 rightTopPos = new Vector2(m_MainRoom.transform.position.x + EnemySpawnPosPositiveOffset.x, m_MainRoom.transform.position.y + EnemySpawnPosPositiveOffset.y);

            m_EnemySpwanPos = new RandomPosition(leftDownPos, rightTopPos, 1f);
        }
    }

    private void Start()
    {
        if (m_MainRoom.GetType() == typeof(RootRoomController))     //检查当前房间是否为初始板块
        {
            m_IsRootRoom = true;        
        }

        //自动给所有此脚本中的的层级赋值
        FurnitureLayerMask = LayerMask.GetMask("Furniture");
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomTrigger.enabled = false;    //玩家进入房间后取消激活门的触发器，防止玩家反复进出房间导致二次生成事件或敌人

            //检查是否为初始房间
            if (!m_IsRootRoom)    
            {
                //游戏处于第一阶段时
                if (!EventManager.Instance.IsSecondStage)
                {
                    //检查该房间是否已经生成过事件，如果没有则生成
                    if (!HasGeneratedEvent)
                    {
                        CloseDoors();     //关门

                        //Debug.Log("An event has generated here: " + transform.position);
                        EventManager.Instance.GenerateRandomEvent(transform.position, this);   //生成事件

                        //房间生成过一次事件后就不会再生成了，因此在这里设置之后，其他地方无需重置布尔值
                        HasGeneratedEvent = true;
                    }                    
                }

                //游戏处于第二阶段时
                else
                {
                    if (!m_HasGeneratedEnemy)       //如果房间没生成过敌人，则生成
                    {
                        CloseDoors();       //关门
                        GenerateEnemy();    //生成敌人

                        m_HasGeneratedEnemy = true;     //普通房间生成过一次敌人后就不会再生成了
                    }                   
                }
            }                       
        }
    }


    //用于设置门的动画器
    private void SetDoorAnimation(bool isOpen)
    {
        foreach(Animator animator in DoorAnimators)
        {
            animator.SetBool("isOpen", isOpen);
            animator.SetBool("isClose", !isOpen);
        }
    }

    public void OpenDoors() => SetDoorAnimation(true);

    private void CloseDoors() => SetDoorAnimation(false);






    public void CheckIfOpenDoors()      //敌人死亡时调用，检查是否达到开门的条件（即房间内所有敌人都死亡）
    {
        if (EnemyObjects.Length != 0)   //先检查房间是否有敌人
        {
            if (KilledEnemyCount >= EnemyObjects.Length)
            {
                OpenDoors();
            }
        }
    }




    //生成敌人后，用Physics2D.Oberlap检测怪物即将生成的坐标是否跟家具重合，如果重合则重新生成坐标
    private void GenerateEnemy()
    {
        if (EnemyObjects.Length != 0)   //如果房间有怪物
        {
            List<Vector2> enemySpawnList = m_EnemySpwanPos.GenerateMultiRandomPos(EnemyObjects.Length);     //根据怪物数量生成随机坐标list

            //生成完坐标列表后。检查列表中是否有跟家具重合的坐标
            CheckIfCollideFurniture(enemySpawnList);
           


            for (int i = 0; i < EnemyObjects.Length; i++)
            {
                //这里的enemy物体是敌人的跟物体（包含巡逻坐标的），在生成的同时赋予物体生成坐标
                GameObject enemyObject = EnemyPool.Instance.GetObject(EnemyObjects[i], enemySpawnList[i]);     //从敌人对象池中生成敌人

                //Debug.Log("The enemy spawn position is : " + enemySpawnList[i]);

                //生成完后重置敌人脚本绑定的物体的本地（相对于父物体）坐标。因为敌人从对象池重新生成后，本地坐标会继承死亡前的本地坐标
                Enemy enemyScript = enemyObject.GetComponentInChildren<Enemy>();

                if (enemyScript != null)
                {
                    enemyScript.ResetLocalPos();
                }


                //设置门控制器的脚本
                enemyScript.SetDoorController(this);

                //生成敌人后重置生命，否则重新激活的敌人生命依然为0
                enemyObject.GetComponentInChildren<Stats>().SetCurrentHealth(enemyObject.GetComponentInChildren<Stats>().MaxHealth);    
            }
        }
    }


    //检查列表中的所有坐标处是否有家具
    private void CheckIfCollideFurniture(List<Vector2> enemySpawnPosList)
    {       
        Vector2 checkSize = new Vector2(m_PhysicsCheckingXPos, m_PhysicsCheckingYPos);      //物理检测的大小

        float adaptiveTolerance = m_EnemySpwanPos.GetOverlapTolerance();        //获取检查重复的距离
        int attemptCount = 0;       //用于防止进入无限循环的变量


        while (attemptCount < 100)      //确保不超过最大尝试次数
        {
            bool isOverlap = false;

            for (int i = 0; i < enemySpawnPosList.Count; i++)
            {
                if (!IsPositionEmpty(enemySpawnPosList[i], checkSize))
                {
                    enemySpawnPosList[i] = m_EnemySpwanPos.GenerateNonOverlappingPosition(enemySpawnPosList);

                    m_EnemySpwanPos.SetOverlapTolerance(adaptiveTolerance);     //设置新的检查重复的距离

                    isOverlap = true;  //设置布尔以继续检查
                }
            }

            if (!isOverlap) break;  //当没有重复时则退出循环

            attemptCount++;
            adaptiveTolerance -= 0.1f;  //如果实在难以生成不会重复的坐标的话，减少检查重复的距离
        }

    }


    //运用物理函数检查要生成的坐标是否有家具
    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 checkSize)
    {
        //第一个参数为中心点，第二个参数为长方形大小（沿着中心各延申一半），第三个参数为角度，第四个参数为检测的目标层级
        Collider2D overlapCheck = Physics2D.OverlapBox(positionToCheck, checkSize, 0f, FurnitureLayerMask);
        return overlapCheck == null;
    }




    public void IncrementEnemyCount()
    {
        KilledEnemyCount++;
        CheckIfOpenDoors();     //增加计数后判断是否满足开门条件

        EnvironmentManager.Instance.IncrementKilledEnemyCount();      //增加记录杀死的敌人数量的整数
    }

    #region Setters
    public void SetHasDeactivateEvent(bool isTrue)
    {
        HasDeactivateEvent = isTrue;
    }
    
    #endregion
}
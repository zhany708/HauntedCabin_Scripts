using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZhangYu.Utilities;

public class DoorController : MonoBehaviour
{
    public Animator[] DoorAnimators;
    public GameObject[] EnemyObjects;
    public LayerMask furnitureLayerMask;
    public Collider2D RoomTrigger {  get; private set; }


    public EventManager EventManagerAtDoor {  get; private set; }


    public int EnemyCount {  get; private set; }
    public bool HasGeneratedEvent { get; private set; }
    public bool HasDeactivateEvent { get; private set; }
    public bool IsRoomClean { get; private set; }     //表示房间中怪物是否清理干净




    RootRoomController m_MainRoom; 
    RandomPosition m_EnemySpwanPos;


    //运用Physics2D检查重复坐标时需要的X和Y的值
    const float m_PhysicsCheckingXPos = 1f;
    const float m_PhysicsCheckingYPos = 2.5f;


    bool m_IsRootRoom;









    private void Awake()
    {
        RoomTrigger = GetComponent<Collider2D>();

        m_MainRoom = GetComponentInParent<RootRoomController>();
        EventManagerAtDoor = FindObjectOfType<EventManager>();      //寻找事件管理器

        /*  当需要房间的两个点时再使用这两个变量
        LeftDownPatrolPoint = new Vector2(m_MainRoom.transform.position.x - 5, m_MainRoom.transform.position.y - 2);
        RightTopPatrolPoint = new Vector2(m_MainRoom.transform.position.x + 5, m_MainRoom.transform.position.y + 2);
        */

        if (EnemyObjects.Length != 0)   //如果房间有怪物
        {
            //敌人生成的x范围为房间坐标的x加减7；生成的y范围为房间坐标的y加1.5，减4
            Vector2 leftDownPos = new Vector2(m_MainRoom.transform.position.x - 7, m_MainRoom.transform.position.y - 4);
            Vector2 rightTopPos = new Vector2(m_MainRoom.transform.position.x + 7, m_MainRoom.transform.position.y + 1.5f);

            m_EnemySpwanPos = new RandomPosition(leftDownPos, rightTopPos);
        }
    }

    private void Start()
    {
        if (m_MainRoom.GetType() == typeof(RootRoomController))     //检查当前房间是否为初始板块
        {
            IsRoomClean = true;
            m_IsRootRoom = true;        
        }
        else
        {
            IsRoomClean = false;
        }


        HasGeneratedEvent = false;
        HasDeactivateEvent = false;
        EnemyCount = 0;

        //自动给所有此脚本中的的层级赋值
        furnitureLayerMask = LayerMask.GetMask("Furniture");
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomTrigger.enabled = false;    //玩家进入房间后取消激活门的触发器，防止玩家反复进出房间导致二次生成事件或敌人

            //检查房间是否清理完毕
            if (!IsRoomClean)
            {
                CloseDoors();

                //检查是否为初始房间
                if (!m_IsRootRoom)    
                {
                    //游戏处于第一阶段时
                    if (!EventManagerAtDoor.IsSecondStage)
                    {
                        //检查是否已经生成过事件，如果没有生成过则继续
                        if (!HasGeneratedEvent)
                        {
                            //Debug.Log("An event has generated here: " + transform.position);
                            EventManagerAtDoor.GenerateRandomEvent(transform.position, this);   //第一阶段时生成事件

                            //房间生成过一次事件后就不会再生成了，因此在这里设置之后，其他地方无需重置布尔值
                            HasGeneratedEvent = true;

                            //如果房间没有怪物需要生成，则生成事件后就干净了
                            if (EnemyObjects.Length == 0)
                            {
                                IsRoomClean = true;
                            }
                        }                    
                    }

                    //游戏处于第二阶段时
                    else
                    {
                        GenerateEnemy();    //只有进入二阶段后才会生成敌人
                    }
                }
            }              
        }
    }



    public void OpenDoors()
    {
        for (int i = 0; i < DoorAnimators.Length; i++)
        {
            DoorAnimators[i].SetBool("IsOpen", true);      //将门打开
            DoorAnimators[i].SetBool("IsClose", false);
        }
    }

    private void CloseDoors()
    {
        for (int i = 0; i < DoorAnimators.Length; i++)
        {
            DoorAnimators[i].SetBool("IsOpen", false);      //将门关闭
            DoorAnimators[i].SetBool("IsClose", true);
        }
    }





    public void CheckIfOpenDoors()      //敌人死亡时调用
    {
        if (EnemyObjects.Length != 0)   //如果房间有怪物
        {
            if (EnemyCount >= EnemyObjects.Length)
            {
                IsRoomClean = true;
                OpenDoors();
            }
        }
    }




    //需要做的：用Physics2D.Oberlap检测怪物即将生成的坐标是否跟家具重合，如果重合则重新生成坐标
    private void GenerateEnemy()
    {
        if (EnemyObjects.Length != 0)   //如果房间有怪物
        {
            List<Vector2> enemySpawnList = m_EnemySpwanPos.GenerateMultiRandomPos(EnemyObjects.Length);     //根据怪物数量生成随机坐标list

            //检查要生成的坐标处是否有家具
            Vector2 checkSize = new Vector2(m_PhysicsCheckingXPos, m_PhysicsCheckingYPos);      //物理检测的大小
            bool isCheckDone = false;     //表示是否检查完
            while(!isCheckDone)
            {
                isCheckDone = true;     //重置布尔

                for (int i = 0; i < enemySpawnList.Count; i++)
                {
                    if (!IsPositionEmpty(enemySpawnList[i], checkSize))
                    {
                        enemySpawnList[i] = m_EnemySpwanPos.GenerateSingleRandomPos();

                        isCheckDone = false;        //设置布尔，从而继续检查
                    }
                }
            }
           


            for (int i = 0; i < EnemyObjects.Length; i++)
            {
                GameObject enemy = EnemyPool.Instance.GetObject(EnemyObjects[i], enemySpawnList[i]);     //从敌人对象池中生成敌人
                enemy.transform.position = enemySpawnList[i];
                enemy.GetComponentInChildren<EnemyDeath>().SetDoorController(this);
                enemy.GetComponentInChildren<Stats>().SetCurrentHealth(enemy.GetComponentInChildren<Stats>().MaxHealth);    //生成敌人后重置生命，否则重新激活的敌人生命依然为0
            }
        }
    }


    //运用物理函数检查要生成的坐标是否有家具
    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 checkSize)
    {
        //第一个参数为中心点，第二个参数为长方形大小，第三个参数为角度，第四个参数为检测的目标层级
        Collider2D overlapCheck = Physics2D.OverlapBox(positionToCheck, checkSize, 0f, furnitureLayerMask);
        return overlapCheck == null;
    }




    public void IncrementEnemyCount()
    {
        EnemyCount++;
    }


    #region Setters
    /*
    public void SetHasGeneratedEvent(bool isTrue)
    {
        HasGeneratedEvent = isTrue;
    }
    */

    public void SetHasDeactivateEvent(bool isTrue)
    {
        HasDeactivateEvent = isTrue;
    }
    
    #endregion
}
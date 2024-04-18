using System.Collections.Generic;
using UnityEngine;
using ZhangYu.Utilities;

public class DoorController : MonoBehaviour
{
    public Animator[] DoorAnimators;
    public GameObject[] EnemyObjects;
    public Collider2D RoomTrigger {  get; private set; }


    public EventManager EventManagerAtDoor {  get; private set; }


    public int EnemyCount {  get; private set; }
    public bool HasGeneratedEvent { get; private set; }
    public bool IsRoomClean { get; private set; }     //表示房间中怪物是否清理干净




    RootRoomController m_MainRoom; 
    RandomPosition m_EnemySpwanPos;


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
            //生成的x范围为房间坐标的x加减5，生成的y范围为房间坐标的y加减2
            m_EnemySpwanPos = new RandomPosition(new Vector2(m_MainRoom.transform.position.x - 5, m_MainRoom.transform.position.y - 2), new Vector2(m_MainRoom.transform.position.x + 5, m_MainRoom.transform.position.y + 2));
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
        EnemyCount = 0;
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomTrigger.enabled = false;    //玩家进入房间后取消激活门的触发器，防止玩家反复进出房间导致二次生成事件或敌人

            if (!IsRoomClean)
            {
                CloseDoors();

                if (!m_IsRootRoom)    
                {
                    if (!EventManagerAtDoor.IsSecondStage)
                    {
                        //Debug.Log(transform.position);
                        EventManagerAtDoor.GenerateRandomEvent(transform.position, this);   //第一阶段时生成事件
                        HasGeneratedEvent = true;
                    }
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





    private void GenerateEnemy()
    {
        if (EnemyObjects.Length != 0)   //如果房间有怪物
        {
            List<Vector2> enemySpawnList = m_EnemySpwanPos.GenerateMultiRandomPos(EnemyObjects.Length);     //根据怪物数量生成随机坐标list

            for (int i = 0; i < EnemyObjects.Length; i++)
            {
                GameObject enemy = EnemyPool.Instance.GetObject(EnemyObjects[i], enemySpawnList[i]);     //从敌人对象池中生成敌人
                enemy.transform.position = enemySpawnList[i];
                enemy.GetComponentInChildren<EnemyDeath>().SetDoorController(this);
                enemy.GetComponentInChildren<Stats>().SetCurrentHealth(enemy.GetComponentInChildren<Stats>().MaxHealth);    //生成敌人后重置生命，否则重新激活的敌人生命依然为0
            }
        }
    }




    public void IncrementEnemyCount()
    {
        EnemyCount++;
    }


    #region Setters
    public void SetHasGeneratedEvent(bool isTrue)
    {
        HasGeneratedEvent = isTrue;
    }
    #endregion
}
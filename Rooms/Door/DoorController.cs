using UnityEngine;
using ZhangYu.Utilities;
using System.Collections.Generic;



public class DoorController : MonoBehaviour
{
    public Animator[] DoorAnimators;        //当前房间内所有门的动画器
    public GameObject[] EnemyObjects;       //当前房间内所有会生成的敌人      需要做的：用词典表示要生成的敌人名字和数量，随后异步加载
    public Vector2 EnemySpawnPosNegativeOffset = Vector2.zero;     //敌人生成的负坐标范围（最左边和最下边的范围，x和y都是负数）
    public Vector2 EnemySpawnPosPositiveOffset = Vector2.zero;     //敌人生成的正坐标范围（最右边和最上边的范围，x和y都是正数）


    //用于储存所有需要永久关闭的门（代替生成木桶阻碍玩家前进）
    public List<string> AlwaysClosedDoorNames { get; private set; } = new List<string>();      
    public Collider2D RoomTrigger { get; private set; }
    public LayerMask FurnitureLayerMask { get; private set; }      //家具的Layer
    public RandomPosition EnemySpwanPos { get; private set; }      //用于随机生成敌人坐标的脚本组件
    public int KilledEnemyCount { get; private set; } = 0;         //表示当前房间内击杀了多少敌人
    public bool HasGeneratedEvent { get; private set; } = false;   //表示当前房间是否生成过事件

    //运用Physics2D检查重复坐标时需要的X和Y的值（火蝙蝠Y轴上有0.5的偏差，因为坐标点位于脚底）
    public float PhysicsCheckingXPos { get; private set; } = 2f;
    public float PhysicsCheckingYPos { get; private set; } = 4f;



    RootRoomController m_MainRoom;          //父物体中用于控制房间的脚本组件

    bool m_IsRootRoom = false;              //表示当前门所在的房间是否为初始房间
    bool m_HasGeneratedEnemy = false;       //表示当前房间是否生成过敌人








    private void Awake()
    {
        RoomTrigger = GetComponent<Collider2D>();

        m_MainRoom = GetComponentInParent<RootRoomController>();

        if (EnemyObjects.Length != 0)   //如果房间有怪物
        {
            //如果这两个变量没有正确赋值，则敌人永远只会固定生成在房间中心
            if (EnemySpawnPosNegativeOffset == Vector2.zero || EnemySpawnPosPositiveOffset == Vector2.zero)
            {
                Debug.LogError("You havn't assigned the EnemySpawnPosOffset correctly in this room: " + m_MainRoom);
                return;
            }


            //敌人生成的x范围为房间坐标的x加变量中的值；生成的y范围为房间坐标的y加变量中的值
            Vector2 leftDownPos = new Vector2(m_MainRoom.transform.position.x + EnemySpawnPosNegativeOffset.x, m_MainRoom.transform.position.y + EnemySpawnPosNegativeOffset.y);
            Vector2 rightTopPos = new Vector2(m_MainRoom.transform.position.x + EnemySpawnPosPositiveOffset.x, m_MainRoom.transform.position.y + EnemySpawnPosPositiveOffset.y);

            EnemySpwanPos = new RandomPosition(leftDownPos, rightTopPos, 1f);
        }
    }

    private void Start()
    {
        //检查当前房间是否为普通房间（不是的话就不生成事件和敌人）
        if ( !(m_MainRoom.GetComponent<RootRoomController>() is NormalRoomController) )
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
                        EnvironmentManager.Instance.GenerateEnemy(this);    //生成敌人

                        m_HasGeneratedEnemy = true;     //普通房间生成过一次敌人后就不会再生成了
                    }                   
                }
            }                       
        }
    }



    //关闭所有永久关闭的门，不影响其他的门
    public void CloseNecessaryDoors()
    {
        foreach (Animator animator in DoorAnimators)
        {
            string doorName = animator.gameObject.name;         //获取门动画器所依附的物体的名字
            if (AlwaysClosedDoorNames.Contains(doorName))
            {
                //确保永久关闭的门始终执行以下逻辑
                animator.SetBool("isOpen", false);
                animator.SetBool("isClose", true);
            }
        }
    }

    //用于代替生成木桶阻碍玩家前进的逻辑（当需要阻碍玩家通过某个门时，获取当前脚本并在列表AlwaysClosedDoorNames中添加该门的名字）
    private void SetDoorAnimation(bool isOpen)
    {      
        foreach (Animator animator in DoorAnimators)
        {
            string doorName = animator.gameObject.name;         //获取门动画器所依附的物体的名字
            if (!AlwaysClosedDoorNames.Contains(doorName))      //只有正常的门才会根据参数进行打开/关闭的逻辑
            {
                animator.SetBool("isOpen", isOpen);
                animator.SetBool("isClose", !isOpen);
            }
            else
            {
                //确保永久关闭的门始终执行以下逻辑
                animator.SetBool("isOpen", false);
                animator.SetBool("isClose", true);
            }
        }
    }

    public void OpenDoors() => SetDoorAnimation(true);

    public void CloseDoors() => SetDoorAnimation(false);

    


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



    public void IncrementEnemyCount()
    {
        KilledEnemyCount++;
        CheckIfOpenDoors();     //增加计数后判断是否满足开门条件

        EnvironmentManager.Instance.IncrementKilledEnemyCount();      //增加记录杀死的敌人数量的整数
    }
}
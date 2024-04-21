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
    public bool IsRoomClean { get; private set; }     //��ʾ�����й����Ƿ�����ɾ�




    RootRoomController m_MainRoom; 
    RandomPosition m_EnemySpwanPos;


    //����Physics2D����ظ�����ʱ��Ҫ��X��Y��ֵ
    const float m_PhysicsCheckingXPos = 1f;
    const float m_PhysicsCheckingYPos = 2.5f;


    bool m_IsRootRoom;









    private void Awake()
    {
        RoomTrigger = GetComponent<Collider2D>();

        m_MainRoom = GetComponentInParent<RootRoomController>();
        EventManagerAtDoor = FindObjectOfType<EventManager>();      //Ѱ���¼�������

        /*  ����Ҫ�����������ʱ��ʹ������������
        LeftDownPatrolPoint = new Vector2(m_MainRoom.transform.position.x - 5, m_MainRoom.transform.position.y - 2);
        RightTopPatrolPoint = new Vector2(m_MainRoom.transform.position.x + 5, m_MainRoom.transform.position.y + 2);
        */

        if (EnemyObjects.Length != 0)   //��������й���
        {
            //�������ɵ�x��ΧΪ���������x�Ӽ�7�����ɵ�y��ΧΪ���������y��1.5����4
            Vector2 leftDownPos = new Vector2(m_MainRoom.transform.position.x - 7, m_MainRoom.transform.position.y - 4);
            Vector2 rightTopPos = new Vector2(m_MainRoom.transform.position.x + 7, m_MainRoom.transform.position.y + 1.5f);

            m_EnemySpwanPos = new RandomPosition(leftDownPos, rightTopPos);
        }
    }

    private void Start()
    {
        if (m_MainRoom.GetType() == typeof(RootRoomController))     //��鵱ǰ�����Ƿ�Ϊ��ʼ���
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

        //�Զ������д˽ű��еĵĲ㼶��ֵ
        furnitureLayerMask = LayerMask.GetMask("Furniture");
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomTrigger.enabled = false;    //��ҽ��뷿���ȡ�������ŵĴ���������ֹ��ҷ����������䵼�¶��������¼������

            //��鷿���Ƿ��������
            if (!IsRoomClean)
            {
                CloseDoors();

                //����Ƿ�Ϊ��ʼ����
                if (!m_IsRootRoom)    
                {
                    //��Ϸ���ڵ�һ�׶�ʱ
                    if (!EventManagerAtDoor.IsSecondStage)
                    {
                        //����Ƿ��Ѿ����ɹ��¼������û�����ɹ������
                        if (!HasGeneratedEvent)
                        {
                            //Debug.Log("An event has generated here: " + transform.position);
                            EventManagerAtDoor.GenerateRandomEvent(transform.position, this);   //��һ�׶�ʱ�����¼�

                            //�������ɹ�һ���¼���Ͳ����������ˣ��������������֮�������ط��������ò���ֵ
                            HasGeneratedEvent = true;

                            //�������û�й�����Ҫ���ɣ��������¼���͸ɾ���
                            if (EnemyObjects.Length == 0)
                            {
                                IsRoomClean = true;
                            }
                        }                    
                    }

                    //��Ϸ���ڵڶ��׶�ʱ
                    else
                    {
                        GenerateEnemy();    //ֻ�н�����׶κ�Ż����ɵ���
                    }
                }
            }              
        }
    }



    public void OpenDoors()
    {
        for (int i = 0; i < DoorAnimators.Length; i++)
        {
            DoorAnimators[i].SetBool("IsOpen", true);      //���Ŵ�
            DoorAnimators[i].SetBool("IsClose", false);
        }
    }

    private void CloseDoors()
    {
        for (int i = 0; i < DoorAnimators.Length; i++)
        {
            DoorAnimators[i].SetBool("IsOpen", false);      //���Źر�
            DoorAnimators[i].SetBool("IsClose", true);
        }
    }





    public void CheckIfOpenDoors()      //��������ʱ����
    {
        if (EnemyObjects.Length != 0)   //��������й���
        {
            if (EnemyCount >= EnemyObjects.Length)
            {
                IsRoomClean = true;
                OpenDoors();
            }
        }
    }




    //��Ҫ���ģ���Physics2D.Oberlap�����Ｔ�����ɵ������Ƿ���Ҿ��غϣ�����غ���������������
    private void GenerateEnemy()
    {
        if (EnemyObjects.Length != 0)   //��������й���
        {
            List<Vector2> enemySpawnList = m_EnemySpwanPos.GenerateMultiRandomPos(EnemyObjects.Length);     //���ݹ������������������list

            //���Ҫ���ɵ����괦�Ƿ��мҾ�
            Vector2 checkSize = new Vector2(m_PhysicsCheckingXPos, m_PhysicsCheckingYPos);      //������Ĵ�С
            bool isCheckDone = false;     //��ʾ�Ƿ�����
            while(!isCheckDone)
            {
                isCheckDone = true;     //���ò���

                for (int i = 0; i < enemySpawnList.Count; i++)
                {
                    if (!IsPositionEmpty(enemySpawnList[i], checkSize))
                    {
                        enemySpawnList[i] = m_EnemySpwanPos.GenerateSingleRandomPos();

                        isCheckDone = false;        //���ò������Ӷ��������
                    }
                }
            }
           


            for (int i = 0; i < EnemyObjects.Length; i++)
            {
                GameObject enemy = EnemyPool.Instance.GetObject(EnemyObjects[i], enemySpawnList[i]);     //�ӵ��˶���������ɵ���
                enemy.transform.position = enemySpawnList[i];
                enemy.GetComponentInChildren<EnemyDeath>().SetDoorController(this);
                enemy.GetComponentInChildren<Stats>().SetCurrentHealth(enemy.GetComponentInChildren<Stats>().MaxHealth);    //���ɵ��˺������������������¼���ĵ���������ȻΪ0
            }
        }
    }


    //�������������Ҫ���ɵ������Ƿ��мҾ�
    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 checkSize)
    {
        //��һ������Ϊ���ĵ㣬�ڶ�������Ϊ�����δ�С������������Ϊ�Ƕȣ����ĸ�����Ϊ����Ŀ��㼶
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
using System.Collections.Generic;
using UnityEngine;
using ZhangYu.Utilities;



public class DoorController : MonoBehaviour
{
    public Animator[] DoorAnimators;
    public GameObject[] EnemyObjects;
    public LayerMask FurnitureLayerMask;
    public Collider2D RoomTrigger {  get; private set; }


    public EventManager EventManagerAtDoor {  get; private set; }


    public int EnemyCount {  get; private set; }
    public bool HasGeneratedEvent { get; private set; }
    public bool HasDeactivateEvent { get; private set; }
    public bool IsRoomClean { get; private set; }     //��ʾ�����й����Ƿ�����ɾ�




    RootRoomController m_MainRoom; 
    RandomPosition m_EnemySpwanPos;


    //����Physics2D����ظ�����ʱ��Ҫ��X��Y��ֵ��������Y������0.5��ƫ���Ϊ�����λ�ڽŵף�
    const float m_PhysicsCheckingXPos = 2f;
    const float m_PhysicsCheckingYPos = 4f;


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
            //�������ɵ�x��ΧΪ���������x�Ӽ�5.5�����ɵ�y��ΧΪ���������y��1.5����3.5
            Vector2 leftDownPos = new Vector2(m_MainRoom.transform.position.x - 5.5f, m_MainRoom.transform.position.y - 3.5f);
            Vector2 rightTopPos = new Vector2(m_MainRoom.transform.position.x + 5.5f, m_MainRoom.transform.position.y + 1.5f);

            m_EnemySpwanPos = new RandomPosition(leftDownPos, rightTopPos, 1f);
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
        FurnitureLayerMask = LayerMask.GetMask("Furniture");
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


    //���������ŵĶ�����
    private void SetDoorState(bool isOpen)
    {
        foreach(Animator animator in DoorAnimators)
        {
            animator.SetBool("IsOpen", isOpen);
            animator.SetBool("IsClose", !isOpen);
        }
    }

    public void OpenDoors() => SetDoorState(true);

    private void CloseDoors() => SetDoorState(false);






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




    //���ɵ��˺���Physics2D.Oberlap�����Ｔ�����ɵ������Ƿ���Ҿ��غϣ�����غ���������������
    private void GenerateEnemy()
    {
        if (EnemyObjects.Length != 0)   //��������й���
        {
            List<Vector2> enemySpawnList = m_EnemySpwanPos.GenerateMultiRandomPos(EnemyObjects.Length);     //���ݹ������������������list

            //�����������б�󡣼���б����Ƿ��и��Ҿ��غϵ�����
            CheckIfCollideFurniture(enemySpawnList);
           


            for (int i = 0; i < EnemyObjects.Length; i++)
            {
                //�����enemy�����ǵ��˵ĸ����壨����Ѳ������ģ��������ɵ�ͬʱ����������������
                GameObject enemyObject = EnemyPool.Instance.GetObject(EnemyObjects[i], enemySpawnList[i]);     //�ӵ��˶���������ɵ���

                //Debug.Log("The enemy spawn position is : " + enemySpawnList[i]);

                //����������õ��˽ű��󶨵�����ı��أ�����ڸ����壩���ꡣ��Ϊ���˴Ӷ�����������ɺ󣬱��������̳�����ǰ�ı�������
                Enemy enemyScript = enemyObject.GetComponentInChildren<Enemy>();

                if (enemyScript != null)
                {
                    enemyScript.ResetLocalPos();
                }
 

                //�����ſ������Ľű�
                enemyObject.GetComponentInChildren<EnemyDeath>().SetDoorController(this);

                //���ɵ��˺������������������¼���ĵ���������ȻΪ0
                enemyObject.GetComponentInChildren<Stats>().SetCurrentHealth(enemyObject.GetComponentInChildren<Stats>().MaxHealth);    
            }
        }
    }


    //����б��е��������괦�Ƿ��мҾ�
    private void CheckIfCollideFurniture(List<Vector2> enemySpawnPosList)
    {       
        Vector2 checkSize = new Vector2(m_PhysicsCheckingXPos, m_PhysicsCheckingYPos);      //������Ĵ�С

        float adaptiveTolerance = m_EnemySpwanPos.GetOverlapTolerance();        //��ȡ����ظ��ľ���
        int attemptCount = 0;       //���ڷ�ֹ��������ѭ���ı���


        while (attemptCount < 100)      //ȷ������������Դ���
        {
            bool isOverlap = false;

            for (int i = 0; i < enemySpawnPosList.Count; i++)
            {
                if (!IsPositionEmpty(enemySpawnPosList[i], checkSize))
                {
                    enemySpawnPosList[i] = m_EnemySpwanPos.GenerateNonOverlappingPosition(enemySpawnPosList);

                    m_EnemySpwanPos.SetOverlapTolerance(adaptiveTolerance);     //�����µļ���ظ��ľ���

                    isOverlap = true;  //���ò����Լ������
                }
            }

            if (!isOverlap) break;  //��û���ظ�ʱ���˳�ѭ��

            attemptCount++;
            adaptiveTolerance -= 0.1f;  //���ʵ���������ɲ����ظ�������Ļ������ټ���ظ��ľ���
        }

    }


    //�������������Ҫ���ɵ������Ƿ��мҾ�
    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 checkSize)
    {
        //��һ������Ϊ���ĵ㣬�ڶ�������Ϊ�����δ�С���������ĸ�����һ�룩������������Ϊ�Ƕȣ����ĸ�����Ϊ����Ŀ��㼶
        Collider2D overlapCheck = Physics2D.OverlapBox(positionToCheck, checkSize, 0f, FurnitureLayerMask);
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
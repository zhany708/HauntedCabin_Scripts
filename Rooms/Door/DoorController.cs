using System.Collections.Generic;
using UnityEngine;
using ZhangYu.Utilities;



public class DoorController : MonoBehaviour
{
    public Animator[] DoorAnimators;        //��ǰ�����������ŵĶ�����
    public GameObject[] EnemyObjects;       //��ǰ���������л����ɵĵ���
    public LayerMask FurnitureLayerMask;    //�Ҿߵ�Layer

    public Vector2 EnemySpawnPosNegativeOffset = Vector2.zero;     //�������ɵĸ����귶Χ������ߺ����±ߵķ�Χ��x��y���Ǹ�����
    public Vector2 EnemySpawnPosPositiveOffset = Vector2.zero;     //�������ɵ������귶Χ�����ұߺ����ϱߵķ�Χ��x��y����������

    public Collider2D RoomTrigger {  get; private set; }


    public int KilledEnemyCount { get; private set; } = 0;          //��ʾ��ǰ�����ڻ�ɱ�˶��ٵ���
    public bool HasGeneratedEvent { get; private set; } = false;    //��ʾ��ǰ�����Ƿ����ɹ��¼�
    public bool HasDeactivateEvent { get; private set; } = false;   //��ʾ��ǰ�����Ƿ����������ɵ��¼�




    RootRoomController m_MainRoom; 
    RandomPosition m_EnemySpwanPos;


    //����Physics2D����ظ�����ʱ��Ҫ��X��Y��ֵ��������Y������0.5��ƫ���Ϊ�����λ�ڽŵף�
    const float m_PhysicsCheckingXPos = 2f;
    const float m_PhysicsCheckingYPos = 4f;


    bool m_IsRootRoom = false;              //��ʾ��ǰ�����ڵķ����Ƿ�Ϊ��ʼ����
    bool m_HasGeneratedEnemy = false;       //��ʾ��ǰ�����Ƿ����ɹ�����








    private void Awake()
    {
        RoomTrigger = GetComponent<Collider2D>();

        m_MainRoom = GetComponentInParent<RootRoomController>();

        if (EnemyObjects.Length != 0)   //��������й���
        {
            //�������ɵ�x��ΧΪ���������x�ӱ����е�ֵ�����ɵ�y��ΧΪ���������y�ӱ����е�ֵ
            Vector2 leftDownPos = new Vector2(m_MainRoom.transform.position.x + EnemySpawnPosNegativeOffset.x, m_MainRoom.transform.position.y + EnemySpawnPosNegativeOffset.y);
            Vector2 rightTopPos = new Vector2(m_MainRoom.transform.position.x + EnemySpawnPosPositiveOffset.x, m_MainRoom.transform.position.y + EnemySpawnPosPositiveOffset.y);

            m_EnemySpwanPos = new RandomPosition(leftDownPos, rightTopPos, 1f);
        }
    }

    private void Start()
    {
        if (m_MainRoom.GetType() == typeof(RootRoomController))     //��鵱ǰ�����Ƿ�Ϊ��ʼ���
        {
            m_IsRootRoom = true;        
        }

        //�Զ������д˽ű��еĵĲ㼶��ֵ
        FurnitureLayerMask = LayerMask.GetMask("Furniture");
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomTrigger.enabled = false;    //��ҽ��뷿���ȡ�������ŵĴ���������ֹ��ҷ����������䵼�¶��������¼������

            //����Ƿ�Ϊ��ʼ����
            if (!m_IsRootRoom)    
            {
                //��Ϸ���ڵ�һ�׶�ʱ
                if (!EventManager.Instance.IsSecondStage)
                {
                    //���÷����Ƿ��Ѿ����ɹ��¼������û��������
                    if (!HasGeneratedEvent)
                    {
                        CloseDoors();     //����

                        //Debug.Log("An event has generated here: " + transform.position);
                        EventManager.Instance.GenerateRandomEvent(transform.position, this);   //�����¼�

                        //�������ɹ�һ���¼���Ͳ����������ˣ��������������֮�������ط��������ò���ֵ
                        HasGeneratedEvent = true;
                    }                    
                }

                //��Ϸ���ڵڶ��׶�ʱ
                else
                {
                    if (!m_HasGeneratedEnemy)       //�������û���ɹ����ˣ�������
                    {
                        CloseDoors();       //����
                        GenerateEnemy();    //���ɵ���

                        m_HasGeneratedEnemy = true;     //��ͨ�������ɹ�һ�ε��˺�Ͳ�����������
                    }                   
                }
            }                       
        }
    }


    //���������ŵĶ�����
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






    public void CheckIfOpenDoors()      //��������ʱ���ã�����Ƿ�ﵽ���ŵ������������������е��˶�������
    {
        if (EnemyObjects.Length != 0)   //�ȼ�鷿���Ƿ��е���
        {
            if (KilledEnemyCount >= EnemyObjects.Length)
            {
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
                enemyScript.SetDoorController(this);

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
        KilledEnemyCount++;
        CheckIfOpenDoors();     //���Ӽ������ж��Ƿ����㿪������

        EnvironmentManager.Instance.IncrementKilledEnemyCount();      //���Ӽ�¼ɱ���ĵ�������������
    }

    #region Setters
    public void SetHasDeactivateEvent(bool isTrue)
    {
        HasDeactivateEvent = isTrue;
    }
    
    #endregion
}
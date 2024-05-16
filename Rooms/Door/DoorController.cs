using UnityEngine;
using ZhangYu.Utilities;



public class DoorController : MonoBehaviour
{
    public Animator[] DoorAnimators;        //��ǰ�����������ŵĶ�����
    public GameObject[] EnemyObjects;       //��ǰ���������л����ɵĵ���
    public Vector2 EnemySpawnPosNegativeOffset = Vector2.zero;     //�������ɵĸ����귶Χ������ߺ����±ߵķ�Χ��x��y���Ǹ�����
    public Vector2 EnemySpawnPosPositiveOffset = Vector2.zero;     //�������ɵ������귶Χ�����ұߺ����ϱߵķ�Χ��x��y����������

    public Collider2D RoomTrigger { get; private set; }
    public LayerMask FurnitureLayerMask { get; private set; }    //�Ҿߵ�Layer
    public RandomPosition EnemySpwanPos { get; private set; }
    public int KilledEnemyCount { get; private set; } = 0;          //��ʾ��ǰ�����ڻ�ɱ�˶��ٵ���
    public bool HasGeneratedEvent { get; private set; } = false;    //��ʾ��ǰ�����Ƿ����ɹ��¼�

    //����Physics2D����ظ�����ʱ��Ҫ��X��Y��ֵ��������Y������0.5��ƫ���Ϊ�����λ�ڽŵף�
    public float PhysicsCheckingXPos { get; private set; } = 2f;
    public float PhysicsCheckingYPos { get; private set; } = 4f;



    RootRoomController m_MainRoom; 

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

            EnemySpwanPos = new RandomPosition(leftDownPos, rightTopPos, 1f);
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
                        EnvironmentManager.Instance.GenerateEnemy(this);    //���ɵ���

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



    public void IncrementEnemyCount()
    {
        KilledEnemyCount++;
        CheckIfOpenDoors();     //���Ӽ������ж��Ƿ����㿪������

        EnvironmentManager.Instance.IncrementKilledEnemyCount();      //���Ӽ�¼ɱ���ĵ�������������
    }
}
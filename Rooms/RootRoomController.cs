using Cinemachine;
using UnityEngine;


public class RootRoomController : MonoBehaviour
{
    /* ������ƿ����
    CinemachineVirtualCamera m_PlayerCamera;
    CinemachineConfiner2D confiner;
    Collider2D m_CameraConfiner;
    */

    public float HiddenTransparency = 0.01f;


    SpriteRenderer[] m_AllSprites;

    DoorController m_DoorInsideThisRoom;
    RoomGenerator m_RoomManager;
    RoomType m_RoomType;


    const float m_DefaultTransparency = 1f;
    bool m_HasGeneratedRoom = false;

    




    private void Awake()
    {
        /* ������ƿ����
        m_PlayerCamera = GameObject.Find("PlayerCamera").GetComponent<CinemachineVirtualCamera>();  //���
        confiner = m_PlayerCamera.GetComponent<CinemachineConfiner2D>();    //����������

        m_CameraConfiner = transform.Find("CameraConfiner").GetComponent<Collider2D>();     //ÿ����������������ײ��
        */

        //��ȡ�������Լ�����������ľ���ͼ���
        m_AllSprites = GetComponentsInChildren<SpriteRenderer>();

        m_DoorInsideThisRoom = GetComponentInChildren<DoorController>();

        m_RoomManager = GameObject.Find("RoomManager").GetComponent<RoomGenerator>();   //Ѱ�ҳ�����������ֵ����壬�������Ӧ�����
        if (m_RoomManager == null)
        {
            Debug.LogError("Cannot find the Gameobject 'RoomManager'.");
        }

        m_RoomType = GetComponent<RoomType>();
    }

    private void Start()
    {
        
        if (m_RoomManager.GetGeneratedRoomNum() < m_RoomManager.GetMaxGeneratedRoomNum() )
        {
            //m_RoomManager.GenerateRoom(transform, m_RoomType);      //��Ϸ��ʼʱ���ɹ̶������ķ���
        }
        
    }



    private void OnEnable()
    {
        //Debug.Log(transform.position);

        //���伤��ʱ�����侫��ͼ���͸��
        ChangeRoomTransparency(HiddenTransparency);

        m_RoomManager.IncrementGeneratedRoomNum();
        m_RoomManager.GeneratedRoomPos.Add(transform.position);     //ÿ�����伤��ʱ������ǰ���������ӽ�List
    }

    private void OnDisable()
    {   
        m_RoomManager.GeneratedRoomPos.Remove(transform.position);  //ÿ������ȡ������ʱ����List���Ƴ���ǰ���������
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player entered room: " + gameObject.name);

            //��ҽ��뷿��󣬽�����͸���ȵ���1
            ChangeRoomTransparency(m_DefaultTransparency);

            //confiner.m_BoundingShape2D = m_CameraConfiner;      //��ҽ��뷿��������������������ײ��

            //������Χ���ɹ�һ�η����Ͳ����������ˣ�����������ò���ֵ
            if (!m_HasGeneratedRoom)
            {
                m_RoomManager.GenerateRoom(transform, m_RoomType);  //ÿ����ҽ��뷿�䣬���ڵ�ǰ������Χ�����µķ���
            }         
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //����뿪����󣬽�������͸��
            ChangeRoomTransparency(HiddenTransparency);


            if(m_DoorInsideThisRoom != null)
            {
                m_DoorInsideThisRoom.RoomTrigger.enabled = true;    //����뿪��������¼����ŵĴ��������Ӷ������֮���ٽ���ʱ���ɵ���

                if (m_DoorInsideThisRoom.HasGeneratedEvent)     //��鷿���Ƿ����ɹ��¼�
                {
                    m_DoorInsideThisRoom.EventManagerAtDoor.DeactivateEventObject();        //����뿪����������¼�����
                }
            }             
        }
    }


    //���ķ��������͸����
    private void ChangeRoomTransparency(float alphaVal)
    {
        foreach(var sprite in m_AllSprites)
        {
            var tempColor = sprite.color;
            tempColor.a = alphaVal;
            sprite.color = tempColor;
        }
    }


    #region Setters
    public void SetHasGeneratorRoom(bool isTrue)
    {
        m_HasGeneratedRoom = isTrue;
    }
    #endregion
}
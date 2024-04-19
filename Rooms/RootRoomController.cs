using Cinemachine;
using UnityEngine;


public class RootRoomController : MonoBehaviour
{
    /* 相机限制框相关
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
        /* 相机限制框相关
        m_PlayerCamera = GameObject.Find("PlayerCamera").GetComponent<CinemachineVirtualCamera>();  //相机
        confiner = m_PlayerCamera.GetComponent<CinemachineConfiner2D>();    //玩家相机限制

        m_CameraConfiner = transform.Find("CameraConfiner").GetComponent<Collider2D>();     //每个房间的相机限制碰撞框
        */

        //获取该物体以及所有子物体的精灵图组件
        m_AllSprites = GetComponentsInChildren<SpriteRenderer>();

        m_DoorInsideThisRoom = GetComponentInChildren<DoorController>();

        m_RoomManager = GameObject.Find("RoomManager").GetComponent<RoomGenerator>();   //寻找场景中这个名字的物体，并获得相应的组件
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
            //m_RoomManager.GenerateRoom(transform, m_RoomType);      //游戏开始时生成固定数量的房间
        }
        
    }



    private void OnEnable()
    {
        //Debug.Log(transform.position);

        //房间激活时将房间精灵图变得透明
        ChangeRoomTransparency(HiddenTransparency);

        m_RoomManager.IncrementGeneratedRoomNum();
        m_RoomManager.GeneratedRoomPos.Add(transform.position);     //每当房间激活时，将当前房间的坐标加进List
    }

    private void OnDisable()
    {   
        m_RoomManager.GeneratedRoomPos.Remove(transform.position);  //每当房间取消激活时，从List中移除当前房间的坐标
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player entered room: " + gameObject.name);

            //玩家进入房间后，将房间透明度调回1
            ChangeRoomTransparency(m_DefaultTransparency);

            //confiner.m_BoundingShape2D = m_CameraConfiner;      //玩家进入房间后更改虚拟相机的相机碰撞框

            //房间周围生成过一次房间后就不会再生成了，因此无需重置布尔值
            if (!m_HasGeneratedRoom)
            {
                m_RoomManager.GenerateRoom(transform, m_RoomType);  //每当玩家进入房间，则在当前房间周围生成新的房间
            }         
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //玩家离开房间后，将房间变得透明
            ChangeRoomTransparency(HiddenTransparency);


            if(m_DoorInsideThisRoom != null)
            {
                m_DoorInsideThisRoom.RoomTrigger.enabled = true;    //玩家离开房间后重新激活门的触发器，从而让玩家之后再进入时生成敌人

                if (m_DoorInsideThisRoom.HasGeneratedEvent)     //检查房间是否生成过事件
                {
                    m_DoorInsideThisRoom.EventManagerAtDoor.DeactivateEventObject();        //玩家离开房间后销毁事件物体
                }
            }             
        }
    }


    //更改房间整体的透明度
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
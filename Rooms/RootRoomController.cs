using UnityEngine;
using UnityEngine.SceneManagement;



public class RootRoomController : MonoBehaviour
{
    //delegate是用于限制引用事件的函数的参数（这里是必须有Vector2参数）
    public delegate void RoomGeneratedHandler(Vector2 roomPosition);
    //当玩家第一次进入一个房间时调用的事件，使用上面的限制。接收方为HellsCall脚本
    public static event RoomGeneratedHandler OnPlayerFirstTimeEnterRoom;      



    //后期处理相关的变量
    public float m_DarkPostProcessColorValue = -250f;
    public float m_PostProcessDuration = 1f;

    //房间下的门控制器脚本
    public DoorController DoorControllerInsideThisRoom { get; private set; }
    //房间下的小地图控制器脚本
    public MiniMapController MiniMapControllerInsideThisRoom { get; private set; }


    const string m_ShadowObjectName = "Shadow";         //阴影图物体的名字
    Transform m_Shadow;                                 //房间的阴影图物体
    
    RoomType m_RoomType;

 

    bool m_HasGeneratedRoom = false;
    bool m_FirstTimeEnterRoom = true;                   //表示玩家是否第一次进入该房间








    #region Unity内部函数
    protected virtual void Awake()
    {
        DoorControllerInsideThisRoom = GetComponentInChildren<DoorController>();
        MiniMapControllerInsideThisRoom = GetComponentInChildren<MiniMapController>();
        m_RoomType = GetComponent<RoomType>();

        //寻找房间阴影物体
        m_Shadow = transform.Find(m_ShadowObjectName);
        if (m_Shadow == null)
        {
            Debug.LogError("Shadow GameObject is not assigned correctly in the " + name);
            return;
        }
    }

    protected virtual void Start()
    {
        //在这里加进字典，防止字典还没实例化就尝试获取引用导致报错
        if (!RoomManager.Instance.GeneratedRoomDict.ContainsKey(transform.position))
        {
            RoomManager.Instance.GeneratedRoomDict.Add(transform.position, gameObject);

            //Debug.Log("Now we have this number of rooms in the dict: " + RoomManager.Instance.GeneratedRoomDict.Count);
        }
    }

    protected virtual void OnEnable()
    {
        //房间激活时隐藏房间
        SetActiveShadowObject(true);
    }

    protected virtual void OnDisable()
    {
        //每当房间取消激活时，从字典中移除当前房间的坐标
        if (RoomManager.Instance.GeneratedRoomDict.ContainsKey(transform.position))
        {
            RoomManager.Instance.GeneratedRoomDict.Remove(transform.position);
        }
    }



    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player entered room: " + gameObject.name);

            //玩家进入房间后，将房间显示出来
            SetActiveShadowObject(false);


            //检查该房间是否在周围生成过房间，当房间周围生成过一次房间后就不会再生成了
            if (!m_HasGeneratedRoom)
            {
                RoomManager.Instance.GenerateRoomAtAround(transform, m_RoomType);    //在当前房间周围生成新的房间
            }

            //检查玩家是否第一次进入房间
            if (m_FirstTimeEnterRoom)
            {
                //顺序很重要，必须先设置布尔再调用事件
                m_FirstTimeEnterRoom = false;

                OnPlayerFirstTimeEnterRoom?.Invoke(transform.position);              //将当前房间的坐标连接到事件        

                //更改精灵图的透明度，用以在小地图中表示已经进入过该房间了      需要做的：先检查当前房间是否为初始房间，随后再使用合适的方法
                MiniMapControllerInsideThisRoom.ChangeSpriteTransparency(true);       
            }

            RoomManager.Instance.CheckIfConnectSurroundingRooms(transform);          //每当玩家进入房间时，检查当前房间是否连接周围的房间  

            //赋值玩家当前所在的房间的坐标
            MiniMapController.CurrentRoomPosPlayerAt = transform.position;

            //检查哪些房间可以在小地图中显示
            MiniMapController.CheckIfDisplayMiniMap();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //玩家离开房间后，隐藏房间
            SetActiveShadowObject(true);


            //确保只在场景持续存在且未卸载时，才会使用DoTeen（否则在场景卸载后会报错）
            if (SceneManager.GetActiveScene().isLoaded)
            {
                //将相机亮度一瞬间的变暗
                PostProcessManager.Instance.DarkenThenBrighten(m_DarkPostProcessColorValue, m_PostProcessDuration);
            }
            

            if (DoorControllerInsideThisRoom != null)
            {
                //玩家离开房间后重新激活门的触发器，从而让玩家之后再进入时生成敌人
                DoorControllerInsideThisRoom.RoomTrigger.enabled = true;                 
            }             
        }
    }
    #endregion


    #region 主要函数
    //用于激活/隐藏房间阴影
    private void SetActiveShadowObject(bool isActive)
    {
        m_Shadow.gameObject.SetActive(isActive);
    }
    #endregion


    #region 其余函数
    //重置游戏
    public virtual void ResetGame()
    {
        m_HasGeneratedRoom = false;
        m_FirstTimeEnterRoom = true;

        MiniMapControllerInsideThisRoom.ResetGame();
    }
    #endregion


    #region Setters
    public void SetHasGenerateRoom(bool isTrue)
    {
        m_HasGeneratedRoom = isTrue;
    }

    public void SetFirstTimeEnterRoom(bool isTrue)
    {
        m_FirstTimeEnterRoom = isTrue;
    }
    #endregion


    #region Getters
    public bool GetHasGenerateRoom()
    {
        return m_HasGeneratedRoom;
    }

    public bool GetFirstTimeEnterRoom()
    {
        return m_FirstTimeEnterRoom;
    }
    #endregion
}
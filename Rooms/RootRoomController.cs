using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class RootRoomController : MonoBehaviour
{
    //delegate是用于限制引用事件的函数的参数（这里是必须有Vector2参数）
    public delegate void RoomGeneratedHandler(Vector2 roomPosition);
    //当玩家第一次进入一个房间时调用的事件，使用上面的限制。接收方为HellsCall脚本
    public static event RoomGeneratedHandler OnPlayerFirstTimeEnterRoom;      


    //玩家离开房间后房间变成的透明度
    public float HiddenTransparency = 0.01f;

    //后期处理相关的变量
    public float m_DarkPostProcessColorValue = -250f;
    public float m_PostProcessDuration = 1f;


    public DoorController DoorControllerInsideThisRoom { get; private set; }


    List<SpriteRenderer> m_AllSprites;
  
    RoomType m_RoomType;


    //默认透明度为1
    const float m_DefaultTransparency = 1f; 

    bool m_HasGeneratedRoom = false;
    bool m_FirstTimeEnterRoom = true;      //表示玩家是否第一次进入该房间





    #region Unity内部函数循环
    protected virtual void Awake()
    {
        //获取该物体以及所有子物体的精灵图组件
        m_AllSprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>() );

        DoorControllerInsideThisRoom = GetComponentInChildren<DoorController>();
        m_RoomType = GetComponent<RoomType>();
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
        //Debug.Log(transform.position);

        //房间激活时将房间精灵图变得透明
        ChangeRoomTransparency(HiddenTransparency);
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

            //玩家进入房间后，将房间透明度调回1
            ChangeRoomTransparency(m_DefaultTransparency);


            //检查该房间是否在周围生成过房间，当房间周围生成过一次房间后就不会再生成了
            if (!m_HasGeneratedRoom)
            {
                RoomManager.Instance.GenerateRoomAtAround(transform, m_RoomType);  //在当前房间周围生成新的房间
            }

            //检查玩家是否第一次进入房间
            if (m_FirstTimeEnterRoom)
            {
                //顺序很重要，必须先设置布尔再调用事件
                m_FirstTimeEnterRoom = false;

                OnPlayerFirstTimeEnterRoom?.Invoke(transform.position);              //将当前房间的坐标连接到事件               
            }

            RoomManager.Instance.CheckIfConnectSurroundingRooms(transform);        //每当玩家进入房间时，检查当前房间是否连接周围的房间          
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //玩家离开房间后，将房间变得透明
            ChangeRoomTransparency(HiddenTransparency);


            //确保只在场景持续存在且未卸载时，才会使用DoTeen（否则在场景卸载后会报错）
            if (SceneManager.GetActiveScene().isLoaded)
            {
                //将相机亮度一瞬间的变暗
                PostProcessController.Instance.DarkenThenBrighten(m_DarkPostProcessColorValue, m_PostProcessDuration);
            }
            

            if (DoorControllerInsideThisRoom != null)
            {
                DoorControllerInsideThisRoom.RoomTrigger.enabled = true;    //玩家离开房间后重新激活门的触发器，从而让玩家之后再进入时生成敌人             
            }             
        }
    }
    #endregion


    #region 其余函数
    //更改房间整体的透明度
    private void ChangeRoomTransparency(float alphaVal)
    {
        foreach(var sprite in m_AllSprites)
        {
            //检查精灵图是否为空，防止报错
            if (sprite != null)
            {
                var tempColor = sprite.color;
                tempColor.a = alphaVal;
                sprite.color = tempColor;
            }          
        }
    }


    //添加新的精灵图到列表
    public void AddNewSpriteRenderers()
    {
        SpriteRenderer[] newSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach(var sprite in newSprites)
        {
            if(!m_AllSprites.Contains(sprite) )
            {
                m_AllSprites.Add(sprite);
            }
        }
    }

    //重置游戏
    public virtual void ResetGame()
    {
        m_HasGeneratedRoom = false;
        m_FirstTimeEnterRoom = true;

        //隐藏房间
        gameObject.SetActive(false);
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
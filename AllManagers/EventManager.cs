using System;
using UnityEngine;
using UnityEngine.SceneManagement;



public class EventManager : ManagerTemplate<EventManager>
{
    public SO_EventKeys EventKeys;
    public int EnterSecondStageCount;      //进入二阶段所需的事件数

    public bool IsSecondStage { get; private set; } = false;



    Animator m_Animator;
    GameObject m_EventPrefab;                                  //事件预制件
    Vector2 m_RoomPosWhereEventOccur;                          //表示事件发生的房间的坐标
    Vector2 m_RoomPosWhereEnterSecondStage = Vector2.zero;     //表示进入二阶段时的房间的坐标

    int m_EventCount = 0;                                      //表示生成过了多少事件
    int m_RandomGeneratedNum = -1;                             //随机生成的数（用于新的事件生成的索引）

    bool m_IsEnterMainMenu = false;                            //表示玩家是否返回了主菜单



    #region Unity内部函数循环
    protected override void Awake()
    {
        base.Awake();

        m_Animator = GetComponent<Animator>();

        if (EnterSecondStageCount <= 0)
        {
            Debug.LogError("The event counts for entering second stage cannot be less or equal to 0.");
            return;
        }
    }

    private async void Start()
    {
        //提前加载剧本背景，但不实例化   
        if (UIManager.Instance.UIKeys != null && !string.IsNullOrEmpty(UIManager.Instance.UIKeys.HellsCallBackground))
        {
            await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.HellsCallBackground);
        }

        else
        {
            Debug.LogError("UIKeys not set or the key for HellsCallBackground is empty.");
            return;
        }
    }
    #endregion


    #region 事件相关
    public async void GenerateRandomEvent(Vector2 position, DoorController thisDoor)
    {
        m_RoomPosWhereEventOccur = position;

        //根据列表的数量随机生成预兆事件    Todo:等事件足够多后需要决定触发预兆事件的频率
        m_RandomGeneratedNum = UnityEngine.Random.Range(0, EventKeys.EvilEventKeys.Count);       
        
        //确认随机索引后尝试异步加载事件
        try
        {
            GameObject loadedEventPrefab = await LoadPrefabAsync(EventKeys.EvilEventKeys[m_RandomGeneratedNum]);       //异步加载事件物体
            if (loadedEventPrefab != null)
            {
                Event loadedEvent = loadedEventPrefab.GetComponent<Event>();

                //使用对象池生成事件预制件（因为使用对象池，所以第一次生成时会二次激活，导致调用两次OnEnable函数）
                m_EventPrefab = ParticlePool.Instance.GetObject(loadedEvent.EventData.EventPrefab);        
            }

            else
            {
                Debug.LogError("Failed to load event: " + EventKeys.EvilEventKeys[m_RandomGeneratedNum]);
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error loading event: " + ex.Message);
        }



        m_EventPrefab.transform.parent.position = m_RoomPosWhereEventOccur;      //赋值事件触发的房间的坐标给事件的父物体（因为对象池的缘故）

        Event eventScript = m_EventPrefab.GetComponent<Event>();
        if (eventScript == null)
        {
            Debug.LogError("Cannot get the Event Script from the GameObject.");
            return;
        }

        eventScript.SetDoor(thisDoor);         //将事件发生的房间传过去
        eventScript.StartEvent();              //开始事件

        //需要做的：开始事件后从列表中移除事件，防止之后重复触发事件
    }


    //取消激活事件物体
    public void DeactivateEventObject()
    {       
        //尝试将事件推回对象池
        if (!ParticlePool.Instance.PushObject(m_EventPrefab) )
        {
            if (m_EventPrefab != null)
            {
                Debug.LogError("Failed to push the event object back to the pool: " + m_EventPrefab.name);
            }         
        }
        
        /*
        //用Addressables释放事件
        if (m_EventPrefab != null)
        {
            string eventName = m_EventPrefab.name;
            if (eventName.EndsWith("(Clone)"))
            {
                //检查是否有“克隆”后缀，如果有的话减去后缀。（Clone）刚好有7个字符
                eventName = eventName.Substring(0, eventName.Length - 7);
            }

            ReleasePrefab(eventName);       //通过Addressables释放内存
        }

        else
        {
            Debug.LogError("Event prefab reference is null, cannot release resources.");
        }  
        */
    }
    #endregion

    
    #region 动画帧事件
    private async void DisplayTransitionStageText()       //用于阶段动画中决定何时生成剧本物体
    {
        //打开剧本
        if (ScreenplayManager.Instance.ScreenplayKeys != null && !string.IsNullOrEmpty(ScreenplayManager.Instance.ScreenplayKeys.HellsCall))
        {
            //确保只在玩家没有返回主菜单时，才进行相关逻辑（否则在返回主菜单后会报错）
            if (!m_IsEnterMainMenu)
            {
                await ScreenplayManager.Instance.OpenScreenplay(ScreenplayManager.Instance.ScreenplayKeys.HellsCall);
            }
        }

        else
        {
            Debug.LogError("ScreenplayKeys not set or HellsCall key is empty.");
        }
    }
    #endregion


    #region 其余函数
    //增加触发过的事件计数
    public void IncrementEventCount()
    {
        m_EventCount++;

        CheckIfTranstionToSecondStage();    //每次事件计数增加后检查是否满足进入二阶段
    }

    //检查是否进入二阶段
    private void CheckIfTranstionToSecondStage()
    {
        if (m_EventCount >= EnterSecondStageCount && !IsSecondStage)   //检查是否触发了足够次数的事件，并且目前不是二阶段
        {
            transform.position = m_RoomPosWhereEventOccur;          //将事件管理器的坐标移到当前房间
            m_RoomPosWhereEnterSecondStage = transform.position;    //储存进入二阶段的房间的坐标
            m_Animator.SetTrigger("TranstionSecondStage");          //随后播放过渡阶段的动画

            IsSecondStage = true;
        }
    }
  

    //每当加载新场景时调用的函数
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //进入主菜单
        if (scene.name == SceneManagerScript.MainMenuSceneName)
        {
            m_IsEnterMainMenu = true;
        }

        //进入一楼场景
        else if (scene.name == SceneManagerScript.FirstFloorSceneName)
        {
            //重置游戏
            ResetGame();
        }

        else
        {
            Debug.Log("We only have two scenes now, please check the parameters!");
        }
    }
    

    //重置游戏
    public void ResetGame()
    {
        //判断玩家是否进行过游戏
        if (m_EventCount != 0 || IsSecondStage)
        {
            //重置触发过的事件
            m_EventCount = 0;
            IsSecondStage = false;          
        }

        m_IsEnterMainMenu = false;
    }
    #endregion


    #region Getters
    public Vector2 GetRoomPosWhereEnterSecondStage()        //获取触发进入二阶段的房间
    {
        return m_RoomPosWhereEnterSecondStage;
    }
    #endregion
}
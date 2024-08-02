using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lean.Localization;



public class EventManager : ManagerTemplate<EventManager>
{
    public SO_EventKeys EventKeys;
    [SerializeField] int m_EnterSecondStageCount = -1;          //进入二阶段所需的事件数

    public bool IsSecondStage { get; private set; } = false;



    //Animator m_Animator;
    GameObject m_EventPrefab;                                   //事件预制件

    NormalRoomController m_RoomWhereEnterSecondStage;           //触发进入二阶段的房间脚本
    Event m_EventWhereEnterSecondStage;                         //触发进入二阶段的事件脚本
    Vector2 m_RoomPosWhereEventOccur;                           //表示事件发生时的房间的坐标
    Vector2 m_RoomPosWhereEnterSecondStage = Vector2.zero;      //表示进入二阶段时的房间的坐标

    int m_EventCount = 0;                                       //表示生成过了多少事件（包括普通和预兆）
    int m_RandomGeneratedNum = -1;                              //随机生成的数（用于新的事件生成的索引）

    bool m_IsEnterMainMenu = false;                             //表示玩家是否返回了主菜单






    #region Unity内部函数循环
    protected override void Awake()
    {
        base.Awake();

        //m_Animator = GetComponent<Animator>();

        if (m_EnterSecondStageCount <= 0)
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


        //提前加载老虎机界面
        if (UIManager.Instance.UIKeys != null && !string.IsNullOrEmpty(UIManager.Instance.UIKeys.SlotMachinePanel))
        {
            await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.SlotMachinePanel);
        }
    }
    #endregion


    #region 事件相关
    public async void GenerateRandomEvent(Vector2 position, DoorController thisDoor)
    {
        m_RoomPosWhereEventOccur = position;

        //根据列表的数量随机生成预兆事件    Todo:等事件足够多后需要决定触发预兆事件的频率
        m_RandomGeneratedNum = UnityEngine.Random.Range(0, EventKeys.DarkEventKeys.Count);       
        
        //确认随机索引后尝试异步加载事件
        try
        {
            GameObject loadedEventPrefab = await LoadPrefabAsync(EventKeys.DarkEventKeys[m_RandomGeneratedNum]);       //异步加载事件物体
            if (loadedEventPrefab != null)
            {
                Event loadedEvent = loadedEventPrefab.GetComponent<Event>();        //获取事件脚本，随后从脚本中的Data处获取事件物体


                //生成警告物体，同时将对应的逻辑传递给物体
                EnvironmentManager.Instance.GenerateSpawnWarningObject(() => LogicPassToSpawnWarningObject(loadedEvent.EventData.EventPrefab, thisDoor)
                , m_RoomPosWhereEventOccur);        
            }

            else
            {
                Debug.LogError("Failed to load event: " + EventKeys.DarkEventKeys[m_RandomGeneratedNum]);
                return;
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error loading event: " + ex.Message);
            return;
        }

        //需要做的：开始事件后从列表中移除事件，防止之后重复触发事件
    }

    //传递给提醒物体脚本的函数，以在提醒完成后生成事件
    private void LogicPassToSpawnWarningObject(GameObject eventObject, DoorController doorController)
    {
        //使用对象池生成事件预制件（因为使用对象池，所以第一次生成时会二次激活，导致调用两次OnEnable函数）
        m_EventPrefab = ParticlePool.Instance.GetObject(eventObject); 


        //赋值事件触发的房间的坐标给事件的父物体（因为对象池的缘故）
        m_EventPrefab.transform.parent.position = m_RoomPosWhereEventOccur;      

        Event eventScript = m_EventPrefab.GetComponent<Event>();
        if (eventScript == null)
        {
            Debug.LogError("Cannot get the Event Script from the " + m_EventPrefab.gameObject.name);
            return;
        }

        eventScript.SetDoor(doorController);            //将事件发生的房间传过去
        eventScript.StartEvent();                       //开始事件
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
    private async void CheckIfTranstionToSecondStage()
    {
        //检查是否触发了足够次数的预兆事件，并且目前不是二阶段
        if (DarkEvent.DarkEventCount >= m_EnterSecondStageCount && !IsSecondStage)   
        {
            m_RoomPosWhereEnterSecondStage = m_RoomPosWhereEventOccur;    //储存进入二阶段的房间的坐标

            SetEventAndRoomCauseSecondStage();                      //获取触发进入二阶段的房间和事件的脚本

            /*
            transform.position = m_RoomPosWhereEventOccur;          //将事件管理器的坐标移到当前房间            
            m_Animator.SetTrigger("TranstionSecondStage");          //随后播放过渡阶段的动画
            */

            await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.SlotMachinePanel);     //打开老虎机界面

            //获取老虎机脚本的引用
            SlotMachinePanel slotMachine = UIManager.Instance.GetUIRoot().GetComponentInChildren<SlotMachinePanel>();
            if (slotMachine == null)
            {
                Debug.LogError($"Cannot get the SlotMachinePanel in the children of {UIManager.Instance.GetUIRoot()}");
                return;
            }


            if (LeanLocalization.CurrentLanguages != null)
            {
                //将触发进入二阶段的房间名，事件名以及进入的剧本名赋值给老虎机界面
                slotMachine.SetTextForChanging(LeanLocalization.GetTranslationText(m_RoomWhereEnterSecondStage.RoomNamePhraseKey)
                    , LeanLocalization.GetTranslationText(m_EventWhereEnterSecondStage.EventNamePhraseKey)
                    , LeanLocalization.GetTranslationText(ScreenplayManager.Instance.ScreenplayNamePhraseKey) );
            }

            slotMachine.OnFadeOutFinished += DisplayTransitionStageText;        //老虎机界面关闭后打开剧本背景界面

            IsSecondStage = true;
        }
    }
  
    //获取触发进入二阶段的房间和事件的脚本
    private void SetEventAndRoomCauseSecondStage()
    {
        m_RoomWhereEnterSecondStage = RoomManager.Instance.GeneratedRoomDict[m_RoomPosWhereEnterSecondStage].GetComponent<NormalRoomController>();
        if (m_RoomWhereEnterSecondStage == null)
        {
            Debug.LogError($"Cannot get the NormalRoomController script from the room at position {m_RoomPosWhereEnterSecondStage}");
            return;
        }


        m_EventWhereEnterSecondStage = m_EventPrefab.GetComponent<Event>();
        if (m_EventWhereEnterSecondStage == null)
        {
            Debug.LogError($"Cannot get the Event script from the event prefab {m_EventPrefab}");
            return;
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
        if (m_EventCount != 0 || DarkEvent.DarkEventCount != 0 || IsSecondStage)
        {
            //重置触发过的事件
            m_EventCount = 0;
            DarkEvent.ResetGame();      //重置触发过的预兆事件
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
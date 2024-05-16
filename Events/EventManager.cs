using System;
using UnityEngine;



public class EventManager : ManagerTemplate<EventManager>
{
    public SO_EventKeys EventKeys;
    public int EnterSecondStageCount;      //进入二阶段所需的事件数

    public bool IsSecondStage { get; private set; } = false;



    Animator m_Animator;
    GameObject m_EventPrefab;       //事件预制件
    Vector2 m_RoomPosition;

    int m_EventCount = 0;                 //生成过多少事件  
    int m_RandomGeneratedNum = -1;         //随机生成的数（用于新的事件生成的索引）






    protected override void Awake()
    {
        base.Awake();

        m_Animator = GetComponent<Animator>();

        if (EnterSecondStageCount <= 0)
        {
            Debug.LogError("The event counts for entering second stage cannot less or equal to 0.");
            return;
        }
    }

    private async void Start()
    {
        //提前加载剧本背景，但不实例化   
        if (UIManager.Instance.UIKeys != null && !string.IsNullOrEmpty(UIManager.Instance.UIKeys.HellsCallPanel))
        {
            await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.HellsCallPanel);
        }

        else
        {
            Debug.LogError("UIKeys not set or TransitionStagePanelKey is empty.");
            return;
        }
    }

 



    public async void GenerateRandomEvent(Vector2 position, DoorController thisDoor)
    {
        m_RoomPosition = position;

        m_RandomGeneratedNum = UnityEngine.Random.Range(0, EventKeys.EvilEventKeys.Count);       //根据列表的数量随机生成预兆事件    Todo:等事件足够多后需要决定触发预兆事件的频率
        
        //确认随机索引后尝试异步加载事件
        try
        {
            GameObject loadedEventPrefab = await LoadPrefabAsync(EventKeys.EvilEventKeys[m_RandomGeneratedNum]);       //异步加载事件物体
            if (loadedEventPrefab != null)
            {
                Event loadedEvent = loadedEventPrefab.GetComponent<Event>();

                m_EventPrefab = ParticlePool.Instance.GetObject(loadedEvent.EventData.EventPrefab);        //使用对象池生成事件预制件
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



        m_EventPrefab.transform.parent.position = m_RoomPosition;      //赋值事件触发的房间的坐标给事件的父物体（因为对象池的缘故）

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





    //检查是否进入二阶段
    private void CheckIfTranstionToSecondStage()
    {
        if (m_EventCount >= EnterSecondStageCount && !IsSecondStage)   //检查是否触发了足够次数的事件，并且目前不是二阶段
        {
            transform.position = m_RoomPosition;        //将事件管理器的坐标移到当前房间
            m_Animator.SetTrigger("TranstionSecondStage");  //随后播放过渡阶段的动画

            IsSecondStage = true;
        }
    }

    #region AnimationEvents
    private async void DisplayTransitionStageText()       //用于阶段动画中决定何时显示文字
    {
        //打开剧本背景界面
        if (UIManager.Instance.UIKeys != null && !string.IsNullOrEmpty(UIManager.Instance.UIKeys.HellsCallPanel))
        {
            await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCallPanel);   //打开剧本背景界面
        }

        else
        {
            Debug.LogError("UIKeys not set or HellsCallPanel key is empty.");
        }
    }
    #endregion

    #region Setters 
    public void SetIsSecondStage(bool isTrue)
    {
        IsSecondStage = isTrue;
    }
    
    public void IncrementEventCount()
    {
        m_EventCount++;

        CheckIfTranstionToSecondStage();    //每次事件计数增加后检查是否满足进入二阶段
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
    }
    #endregion
}
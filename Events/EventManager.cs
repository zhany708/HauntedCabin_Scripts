using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    public SO_EventKeys AllEvents;


    public bool IsSecondStage {  get; private set; }



    Animator m_Animator;
    GameObject m_EventPrefab;
    Vector2 m_RoomPosition;


    int EventCount;
    int EnterSecondStageCount;




    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        EventCount = 0;
        EnterSecondStageCount = 1;
        IsSecondStage = false;

        //提前加载进入二阶段的文字
        StartCoroutine(UIManager.Instance.InitPanel(UIConst.TransitionStagePanel) );     
    }

    


    private async Task<Event> LoadEventAsync(string name)
    {
        GameObject loadedEventObject = await Addressables.LoadAssetAsync<GameObject>(name).Task;
        Event loadedEvent = null;
        if (loadedEventObject != null)
        {
            loadedEvent = loadedEventObject.GetComponent<Event>();
        }

        else
        {
            Debug.LogError("Failed to load event: " + name);
        }

        return loadedEvent;
    }


    public async void GenerateRandomEvent(Vector2 position, DoorController thisDoor)
    {
        m_RoomPosition = position;

        int index = UnityEngine.Random.Range(0, AllEvents.Keys.Count);       //随机生成事件
        
        //确认随机索引后尝试异步加载
        try
        {
            Event loadedEvent = await LoadEventAsync(AllEvents.Keys[index]);       //异步加载事件
            if (loadedEvent != null)
            {
                m_EventPrefab = ParticlePool.Instance.GetObject(loadedEvent.EventData.EventPrefab);        //生成事件预制件
            }

            else
            {
                Debug.LogError("Failed to load event: " + AllEvents.Keys[index]);
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error playing music: " + ex.Message);
        }



        m_EventPrefab.transform.parent.position = m_RoomPosition;      //赋值事件触发的房间坐标给事件的父物体（因为对象池的缘故）
        m_EventPrefab.GetComponent<Event>().SetDoor(thisDoor);        //将事件发生的房间传过去
        m_EventPrefab.GetComponent<Event>().SetEventManager(this);       //将当前脚本传给Event脚本
        m_EventPrefab.GetComponent<Event>().StartEvent();       //开始事件

        //AllEvents.RemoveAt(index);      //开始事件后从List中移除事件，防止之后重复触发事件
    }

    public void DeactivateEventObject()
    {
        ParticlePool.Instance.PushObject(m_EventPrefab);    //将事件放回对象池
    }









    private void CheckIfTranstionToSecondStage()
    {
        if (EventCount >= EnterSecondStageCount)
        {
            transform.position = m_RoomPosition;        //将事件管理器的坐标移到当前房间
            m_Animator.SetTrigger("TranstionSecondStage");  //随后播放过渡阶段的动画

            IsSecondStage = true;
        }
    }

    #region AnimationEvents
    private void DisplayTransitionStageText()       //用于阶段动画中决定何时显示文字
    {
        StartCoroutine(UIManager.Instance.OpenPanel(UIConst.TransitionStagePanel) );
    }
    #endregion

    #region Setters
    
    public void SetIsSecondStage(bool isTrue)
    {
        IsSecondStage = isTrue;
    }
    

    public void IncrementEventCount()
    {
        EventCount++;

        CheckIfTranstionToSecondStage();    //每次事件计数增加后检查是否满足进入二阶段
    }
    #endregion
}

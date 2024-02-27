using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public List<SO_EventData> AllEvents;


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
    }

    




    public void GenerateRandomEvent(Vector2 position, DoorController thisDoor)
    {
        m_RoomPosition = position;

        int index = Random.Range(0, AllEvents.Count);       //随机生成事件
        SO_EventData eventData = AllEvents[index];

        m_EventPrefab = ParticlePool.Instance.GetObject(eventData.EventPrefab);        //生成事件预制件


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
        UIManager.Instance.OpenPanel(UIConst.TransitionStagePanel);
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

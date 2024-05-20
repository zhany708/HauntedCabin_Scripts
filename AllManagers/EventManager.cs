using System;
using UnityEngine;



public class EventManager : ManagerTemplate<EventManager>
{
    public SO_EventKeys EventKeys;
    public int EnterSecondStageCount;      //������׶�������¼���

    public bool IsSecondStage { get; private set; } = false;



    Animator m_Animator;
    GameObject m_EventPrefab;       //�¼�Ԥ�Ƽ�
    Vector2 m_RoomPosition;         //��ʾ�¼������ķ��������

    int m_EventCount = 0;                 //���ɹ������¼�  
    int m_RandomGeneratedNum = -1;         //������ɵ����������µ��¼����ɵ�������





    #region Unity�ڲ�����ѭ��
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
        //��ǰ���ؾ籾����������ʵ����   
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
    #endregion


    #region �¼����
    public async void GenerateRandomEvent(Vector2 position, DoorController thisDoor)
    {
        m_RoomPosition = position;

        m_RandomGeneratedNum = UnityEngine.Random.Range(0, EventKeys.EvilEventKeys.Count);       //�����б�������������Ԥ���¼�    Todo:���¼��㹻�����Ҫ��������Ԥ���¼���Ƶ��
        
        //ȷ��������������첽�����¼�
        try
        {
            GameObject loadedEventPrefab = await LoadPrefabAsync(EventKeys.EvilEventKeys[m_RandomGeneratedNum]);       //�첽�����¼�����
            if (loadedEventPrefab != null)
            {
                Event loadedEvent = loadedEventPrefab.GetComponent<Event>();

                m_EventPrefab = ParticlePool.Instance.GetObject(loadedEvent.EventData.EventPrefab);        //ʹ�ö���������¼�Ԥ�Ƽ�
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



        m_EventPrefab.transform.parent.position = m_RoomPosition;      //��ֵ�¼������ķ����������¼��ĸ����壨��Ϊ����ص�Ե�ʣ�

        Event eventScript = m_EventPrefab.GetComponent<Event>();
        if (eventScript == null)
        {
            Debug.LogError("Cannot get the Event Script from the GameObject.");
            return;
        }

        eventScript.SetDoor(thisDoor);         //���¼������ķ��䴫��ȥ
        eventScript.StartEvent();              //��ʼ�¼�

        //��Ҫ���ģ���ʼ�¼�����б����Ƴ��¼�����ֹ֮���ظ������¼�
    }


    //ȡ�������¼�����
    public void DeactivateEventObject()
    {       
        //���Խ��¼��ƻض����
        if (!ParticlePool.Instance.PushObject(m_EventPrefab) )
        {
            if (m_EventPrefab != null)
            {
                Debug.LogError("Failed to push the event object back to the pool: " + m_EventPrefab.name);
            }         
        }
        
        /*
        //��Addressables�ͷ��¼�
        if (m_EventPrefab != null)
        {
            string eventName = m_EventPrefab.name;
            if (eventName.EndsWith("(Clone)"))
            {
                //����Ƿ��С���¡����׺������еĻ���ȥ��׺����Clone���պ���7���ַ�
                eventName = eventName.Substring(0, eventName.Length - 7);
            }

            ReleasePrefab(eventName);       //ͨ��Addressables�ͷ��ڴ�
        }

        else
        {
            Debug.LogError("Event prefab reference is null, cannot release resources.");
        }  
        */
    }
    #endregion



    //����Ƿ������׶�
    private void CheckIfTranstionToSecondStage()
    {
        if (m_EventCount >= EnterSecondStageCount && !IsSecondStage)   //����Ƿ񴥷����㹻�������¼�������Ŀǰ���Ƕ��׶�
        {
            transform.position = m_RoomPosition;            //���¼��������������Ƶ���ǰ����
            m_Animator.SetTrigger("TranstionSecondStage");  //��󲥷Ź��ɽ׶εĶ���

            IsSecondStage = true;
        }
    }


    #region AnimationEvents
    private async void DisplayTransitionStageText()       //���ڽ׶ζ����о�����ʱ���ɾ籾����
    {
        //�򿪾籾
        if (ScreenplayManager.Instance.ScreenplayKeys != null && !string.IsNullOrEmpty(ScreenplayManager.Instance.ScreenplayKeys.HellsCall))
        {
            await ScreenplayManager.Instance.OpenScreenplay(ScreenplayManager.Instance.ScreenplayKeys.HellsCall);
        }

        else
        {
            Debug.LogError("ScreenplayKeys not set or HellsCall key is empty.");
        }
    }
    #endregion


    #region Setters 
    public void IncrementEventCount()
    {
        m_EventCount++;

        CheckIfTranstionToSecondStage();    //ÿ���¼��������Ӻ����Ƿ����������׶�
    }

    //������Ϸ
    public void ResetGame()
    {
        //�ж�����Ƿ���й���Ϸ
        if (m_EventCount != 0 || IsSecondStage)
        {
            //���ô��������¼�
            m_EventCount = 0;
            IsSecondStage = false;
        }
    }
    #endregion
}
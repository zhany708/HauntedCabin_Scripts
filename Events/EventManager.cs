using System;
using UnityEngine;



public class EventManager : ManagerTemplate<EventManager>
{
    [SerializeField]
    public SO_EventKeys EventKeys;


    public bool IsSecondStage {  get; private set; }



    Animator m_Animator;
    GameObject m_EventPrefab;       //�¼�Ԥ�Ƽ�
    Vector2 m_RoomPosition;



    int m_EventCount;                 //���ɹ������¼�
    int m_EnterSecondStageCount;      //������׶�������¼���
    int m_RandomGeneratedNum;         //������ɵ����������µ��¼����ɵ�������



    protected override void Awake()
    {
        base.Awake();

        m_Animator = GetComponent<Animator>();
    }

    private async void Start()
    {
        //��ʼ����������ֹ����Bug
        m_EventCount = 0;
        m_EnterSecondStageCount = 1;
        m_RandomGeneratedNum = -1;
        IsSecondStage = false;


        //��ǰ���ؽ�����׶ε����֣�����ʵ����   
        if (UIManager.Instance.UIKeys != null && !string.IsNullOrEmpty(UIManager.Instance.UIKeys.TransitionStagePanelKey))
        {
            await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.TransitionStagePanelKey);
        }

        else
        {
            Debug.LogError("UIKeys not set or TransitionStagePanelKey is empty.");
        }
    }

 



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
        

        //��Addressables�ͷ��¼�
        if (m_EventPrefab != null)
        {
            string eventName = m_EventPrefab.name;
            if (eventName.EndsWith("(Clone)"))
            {
                //����Ƿ��С���¡����׺������еĻ���ȥ��׺����Clone���պ���7���ַ�
                eventName = eventName.Substring(0, eventName.Length - 7);
            }

            ReleasePrefab(eventName);
        }

        else
        {
            Debug.LogError("Event prefab reference is null, cannot release resources.");
        }  
    }





    //����Ƿ������׶�
    private void CheckIfTranstionToSecondStage()
    {
        if (m_EventCount >= m_EnterSecondStageCount)
        {
            transform.position = m_RoomPosition;        //���¼��������������Ƶ���ǰ����
            m_Animator.SetTrigger("TranstionSecondStage");  //��󲥷Ź��ɽ׶εĶ���

            IsSecondStage = true;
        }
    }

    #region AnimationEvents
    private async void DisplayTransitionStageText()       //���ڽ׶ζ����о�����ʱ��ʾ����
    {
        //��ʾת�׶�����
        if (UIManager.Instance.UIKeys != null && !string.IsNullOrEmpty(UIManager.Instance.UIKeys.TransitionStagePanelKey))
        {
            await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.TransitionStagePanelKey);
        }

        else
        {
            Debug.LogError("UIKeys not set or TransitionStagePanelKey is empty.");
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
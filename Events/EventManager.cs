using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    public SO_EventKeys EventKeys;
    public SO_UIKeys UIKeys;


    public bool IsSecondStage {  get; private set; }



    Animator m_Animator;
    GameObject m_EventPrefab;       //�¼�Ԥ�Ƽ�
    Vector2 m_RoomPosition;

    //������ع����¼�
    Dictionary<string, GameObject> m_EventDict;

    int m_EventCount;                 //���ɹ����ٷ���
    int m_EnterSecondStageCount;      //������׶�����ķ�����
    int m_RandomGeneratedNum;         //������ɵ����������µķ������ɵ�������



    private void Awake()
    {
        m_Animator = GetComponent<Animator>();

        m_EventDict = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        //��ʼ����������ֹ����Bug
        m_EventCount = 0;
        m_EnterSecondStageCount = 1;
        m_RandomGeneratedNum = -1;
        IsSecondStage = false;


        //��ǰ���ؽ�����׶ε����֣�����ʵ����   
        if (UIKeys != null && !string.IsNullOrEmpty(UIKeys.TransitionStagePanelKey))
        {
            StartCoroutine(UIManager.Instance.InitPanel(UIKeys.TransitionStagePanelKey));
        }

        else
        {
            Debug.LogError("UIKeys not set or TransitionStagePanelKey is empty.");
        }
    }

    





    private async Task<Event> LoadEventAsync(string name)
    {
        
        //����ֵ����Ѿ����¼��ˣ���ֱ�ӷ���
        if (m_EventDict.TryGetValue(name, out GameObject thisGameObject))
        {
            Event eventComponent = thisGameObject.GetComponent<Event>();
            return eventComponent;
        }
        

        //�첽�����¼��󣬻�ȡEvent�������󷵻ظ����
        GameObject loadedEventObject = await Addressables.LoadAssetAsync<GameObject>(name).Task;
        Event loadedEvent = null;
        if (loadedEventObject != null)
        {
            //�������ϻ�ȡEvent���
            loadedEvent = loadedEventObject.GetComponent<Event>();

            //��Event����������ֵ�
            m_EventDict[name] = loadedEventObject;
        }

        else
        {
            Debug.LogError("Failed to load event: " + name);
        }

        return loadedEvent;
    }


    //��Addressables���ͷ��¼���ֻ�����������ͷ��ڴ�
    public void ReleaseEvent(string key)
    {
        if (key.EndsWith("(Clone)") )
        {
            //����Ƿ��С���¡����׺������еĻ���ȥ��׺����Clone���պ���7���ַ�
            key = key.Substring(0, key.Length - 7);
        }


        if (m_EventDict.TryGetValue(key, out GameObject eventPrefab))
        {
            Addressables.Release(eventPrefab);

            //���ֵ����Ƴ�����
            m_EventDict.Remove(key);

            Debug.Log("Event released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This event is not loaded yet, cannot release: " + key);
        }
    }




    public async void GenerateRandomEvent(Vector2 position, DoorController thisDoor)
    {
        m_RoomPosition = position;

        m_RandomGeneratedNum = UnityEngine.Random.Range(0, EventKeys.EvilEventKeys.Count);       //�����б�������������Ԥ���¼�    Todo:���¼��㹻�����Ҫ��������Ԥ���¼���Ƶ��
        
        //ȷ��������������첽�����¼�
        try
        {
            Event loadedEvent = await LoadEventAsync(EventKeys.EvilEventKeys[m_RandomGeneratedNum]);       //�첽�����¼�
            if (loadedEvent != null)
            {
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
        m_EventPrefab.GetComponent<Event>().SetDoor(thisDoor);         //���¼������ķ��䴫��ȥ
        m_EventPrefab.GetComponent<Event>().SetEventManager(this);     //����ǰ�ű�����Event�ű�
        m_EventPrefab.GetComponent<Event>().StartEvent();              //��ʼ�¼�

        //AllEvents.EvilEventKeys.RemoveAt(index);      //��ʼ�¼�����б����Ƴ��¼�����ֹ֮���ظ������¼�
    }


    //ȡ�������¼�����
    public void DeactivateEventObject()
    {
        
        //���Խ��¼��ƻض����
        if (!ParticlePool.Instance.PushObject(m_EventPrefab) )
        {
            Debug.LogError("Failed to push the event object back to the pool: " + m_EventPrefab.name);
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

            ReleaseEvent(eventName);
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
    private void DisplayTransitionStageText()       //���ڽ׶ζ����о�����ʱ��ʾ����
    {
        //��ʾת�׶�����
        if (UIKeys != null && !string.IsNullOrEmpty(UIKeys.TransitionStagePanelKey))
        {
            StartCoroutine(UIManager.Instance.OpenPanel(UIKeys.TransitionStagePanelKey));
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
    #endregion
}

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

        //��ǰ���ؽ�����׶ε�����
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

        int index = UnityEngine.Random.Range(0, AllEvents.Keys.Count);       //��������¼�
        
        //ȷ��������������첽����
        try
        {
            Event loadedEvent = await LoadEventAsync(AllEvents.Keys[index]);       //�첽�����¼�
            if (loadedEvent != null)
            {
                m_EventPrefab = ParticlePool.Instance.GetObject(loadedEvent.EventData.EventPrefab);        //�����¼�Ԥ�Ƽ�
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



        m_EventPrefab.transform.parent.position = m_RoomPosition;      //��ֵ�¼������ķ���������¼��ĸ����壨��Ϊ����ص�Ե�ʣ�
        m_EventPrefab.GetComponent<Event>().SetDoor(thisDoor);        //���¼������ķ��䴫��ȥ
        m_EventPrefab.GetComponent<Event>().SetEventManager(this);       //����ǰ�ű�����Event�ű�
        m_EventPrefab.GetComponent<Event>().StartEvent();       //��ʼ�¼�

        //AllEvents.RemoveAt(index);      //��ʼ�¼����List���Ƴ��¼�����ֹ֮���ظ������¼�
    }

    public void DeactivateEventObject()
    {
        ParticlePool.Instance.PushObject(m_EventPrefab);    //���¼��Żض����
    }









    private void CheckIfTranstionToSecondStage()
    {
        if (EventCount >= EnterSecondStageCount)
        {
            transform.position = m_RoomPosition;        //���¼��������������Ƶ���ǰ����
            m_Animator.SetTrigger("TranstionSecondStage");  //��󲥷Ź��ɽ׶εĶ���

            IsSecondStage = true;
        }
    }

    #region AnimationEvents
    private void DisplayTransitionStageText()       //���ڽ׶ζ����о�����ʱ��ʾ����
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

        CheckIfTranstionToSecondStage();    //ÿ���¼��������Ӻ����Ƿ����������׶�
    }
    #endregion
}

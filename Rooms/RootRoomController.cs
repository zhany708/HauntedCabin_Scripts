using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class RootRoomController : MonoBehaviour
{
    //����뿪����󷿼��ɵ�͸����
    public float HiddenTransparency = 0.01f;

    //���ڴ�����صı���
    public float m_DarkPostProcessColorValue = -250f;
    public float m_PostProcessDuration = 1f;


    List<SpriteRenderer> m_AllSprites;


    DoorController m_DoorInsideThisRoom;
    RoomManager m_RoomManager;
    RoomType m_RoomType;
    PostProcessController m_PostProcessController;

    //Ĭ��͸����Ϊ1
    const float m_DefaultTransparency = 1f; 

    bool m_HasGeneratedRoom = false;


    




    private void Awake()
    {
        //��ȡ�������Լ�����������ľ���ͼ���
        m_AllSprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>() );

        m_DoorInsideThisRoom = GetComponentInChildren<DoorController>();

        m_RoomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();   //Ѱ�ҳ�����������ֵ����壬�������Ӧ�����
        if (m_RoomManager == null)
        {
            Debug.LogError("Cannot find the Gameobject 'RoomManager'.");
        }

        m_PostProcessController = GameObject.Find("PostProcess").GetComponent<PostProcessController>();
        if (m_PostProcessController == null)
        {
            Debug.LogError("Cannot find the Gameobject 'PostProcess'.");
        }


        m_RoomType = GetComponent<RoomType>();
    }



    private void OnEnable()
    {
        //Debug.Log(transform.position);

        //���伤��ʱ�����侫��ͼ���͸��
        ChangeRoomTransparency(HiddenTransparency);

        
        //ÿ�����伤��ʱ������ǰ���������ӽ��ֵ�
        if (!m_RoomManager.GeneratedRoomDict.ContainsKey(transform.position) )
        {
            m_RoomManager.GeneratedRoomDict.Add(transform.position, gameObject);
        }          
    }

    private void OnDisable()
    {
        //ÿ������ȡ������ʱ�����ֵ����Ƴ���ǰ���������
        if (m_RoomManager.GeneratedRoomDict.ContainsKey(transform.position))
        {
            m_RoomManager.GeneratedRoomDict.Remove(transform.position);
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player entered room: " + gameObject.name);

            //��ҽ��뷿��󣬽�����͸���ȵ���1
            ChangeRoomTransparency(m_DefaultTransparency);


            //������Χ���ɹ�һ�η����Ͳ�����������
            if (!m_HasGeneratedRoom)
            {
                m_RoomManager.GenerateRoom(transform, m_RoomType);  //�ڵ�ǰ������Χ�����µķ���
            }

            //RoomManager.Instance.CheckIfConnectSurroundingRooms(transform);  //ÿ����ҽ��뷿��ʱ����鵱ǰ�����Ƿ�������Χ�ķ���
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //����뿪����󣬽�������͸��
            ChangeRoomTransparency(HiddenTransparency);


            //ȷ��ֻ�ڳ�������������δж��ʱ���Ż�ʹ��DoTeen�������ڳ���ж�غ�ᱨ��
            if (SceneManager.GetActiveScene().isLoaded)
            {
                //���������һ˲��ı䰵
                m_PostProcessController.DarkenThenBrighten(m_DarkPostProcessColorValue, m_PostProcessDuration);
            }
            

            if (m_DoorInsideThisRoom != null)
            {
                m_DoorInsideThisRoom.RoomTrigger.enabled = true;    //����뿪��������¼����ŵĴ��������Ӷ������֮���ٽ���ʱ���ɵ���             
            }             
        }
    }




    //���ķ��������͸����
    private void ChangeRoomTransparency(float alphaVal)
    {
        foreach(var sprite in m_AllSprites)
        {
            //��龫��ͼ�Ƿ�Ϊ�գ���ֹ����
            if (sprite != null)
            {
                var tempColor = sprite.color;
                tempColor.a = alphaVal;
                sprite.color = tempColor;
            }          
        }
    }


    //����µľ���ͼ���б�
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


    #region Setters
    public void SetHasGeneratorRoom(bool isTrue)
    {
        m_HasGeneratedRoom = isTrue;
    }
    #endregion
}
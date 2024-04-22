using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RootRoomController : MonoBehaviour
{
    public float HiddenTransparency = 0.01f;


    List<SpriteRenderer> m_AllSprites;

    DoorController m_DoorInsideThisRoom;
    RoomGenerator m_RoomManager;
    RoomType m_RoomType;


    const float m_DefaultTransparency = 1f;
    bool m_HasGeneratedRoom = false;

    




    private void Awake()
    {
        //��ȡ�������Լ�����������ľ���ͼ���
        m_AllSprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>() );

        m_DoorInsideThisRoom = GetComponentInChildren<DoorController>();

        m_RoomManager = GameObject.Find("RoomManager").GetComponent<RoomGenerator>();   //Ѱ�ҳ�����������ֵ����壬�������Ӧ�����
        if (m_RoomManager == null)
        {
            Debug.LogError("Cannot find the Gameobject 'RoomManager'.");
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
                m_RoomManager.GenerateRoom(transform, m_RoomType);  //����ҽ��뷿��ʱ���ڵ�ǰ������Χ�����µķ���
            }         
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //����뿪����󣬽�������͸��
            ChangeRoomTransparency(HiddenTransparency);

            if(m_DoorInsideThisRoom != null)
            {
                m_DoorInsideThisRoom.RoomTrigger.enabled = true;    //����뿪��������¼����ŵĴ��������Ӷ������֮���ٽ���ʱ���ɵ���

                if (m_DoorInsideThisRoom.HasGeneratedEvent && !m_DoorInsideThisRoom.HasDeactivateEvent)     //����������ɹ��¼����һ�ûȡ��������¼�ʱ
                {
                    m_DoorInsideThisRoom.EventManagerAtDoor.DeactivateEventObject();        //����뿪����������¼�����
                    m_DoorInsideThisRoom.SetHasDeactivateEvent(true);                                                                            
                }
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
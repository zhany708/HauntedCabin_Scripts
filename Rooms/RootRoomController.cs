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
        //获取该物体以及所有子物体的精灵图组件
        m_AllSprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>() );

        m_DoorInsideThisRoom = GetComponentInChildren<DoorController>();

        m_RoomManager = GameObject.Find("RoomManager").GetComponent<RoomGenerator>();   //寻找场景中这个名字的物体，并获得相应的组件
        if (m_RoomManager == null)
        {
            Debug.LogError("Cannot find the Gameobject 'RoomManager'.");
        }

        m_RoomType = GetComponent<RoomType>();
    }



    private void OnEnable()
    {
        //Debug.Log(transform.position);

        //房间激活时将房间精灵图变得透明
        ChangeRoomTransparency(HiddenTransparency);

        
        //每当房间激活时，将当前房间的坐标加进字典
        if (!m_RoomManager.GeneratedRoomDict.ContainsKey(transform.position) )
        {
            m_RoomManager.GeneratedRoomDict.Add(transform.position, gameObject);
        }          
    }

    private void OnDisable()
    {
        //每当房间取消激活时，从字典中移除当前房间的坐标
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

            //玩家进入房间后，将房间透明度调回1
            ChangeRoomTransparency(m_DefaultTransparency);


            //房间周围生成过一次房间后就不会再生成了
            if (!m_HasGeneratedRoom)
            {
                m_RoomManager.GenerateRoom(transform, m_RoomType);  //当玩家进入房间时，在当前房间周围生成新的房间
            }         
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //玩家离开房间后，将房间变得透明
            ChangeRoomTransparency(HiddenTransparency);

            if(m_DoorInsideThisRoom != null)
            {
                m_DoorInsideThisRoom.RoomTrigger.enabled = true;    //玩家离开房间后重新激活门的触发器，从而让玩家之后再进入时生成敌人

                if (m_DoorInsideThisRoom.HasGeneratedEvent && !m_DoorInsideThisRoom.HasDeactivateEvent)     //如果房间生成过事件，且还没取消激活该事件时
                {
                    m_DoorInsideThisRoom.EventManagerAtDoor.DeactivateEventObject();        //玩家离开房间后销毁事件物体
                    m_DoorInsideThisRoom.SetHasDeactivateEvent(true);                                                                            
                }
            }             
        }
    }




    //更改房间整体的透明度
    private void ChangeRoomTransparency(float alphaVal)
    {
        foreach(var sprite in m_AllSprites)
        {
            var tempColor = sprite.color;
            tempColor.a = alphaVal;
            sprite.color = tempColor;
        }
    }


    //添加新的精灵图到列表
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
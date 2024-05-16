using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class RootRoomController : MonoBehaviour
{
    //玩家离开房间后房间变成的透明度
    public float HiddenTransparency = 0.01f;

    //后期处理相关的变量
    public float m_DarkPostProcessColorValue = -250f;
    public float m_PostProcessDuration = 1f;


    List<SpriteRenderer> m_AllSprites;


    DoorController m_DoorInsideThisRoom;
    RoomManager m_RoomManager;
    RoomType m_RoomType;
    PostProcessController m_PostProcessController;

    //默认透明度为1
    const float m_DefaultTransparency = 1f; 

    bool m_HasGeneratedRoom = false;


    




    private void Awake()
    {
        //获取该物体以及所有子物体的精灵图组件
        m_AllSprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>() );

        m_DoorInsideThisRoom = GetComponentInChildren<DoorController>();

        m_RoomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();   //寻找场景中这个名字的物体，并获得相应的组件
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
                m_RoomManager.GenerateRoom(transform, m_RoomType);  //在当前房间周围生成新的房间
            }

            //RoomManager.Instance.CheckIfConnectSurroundingRooms(transform);  //每当玩家进入房间时，检查当前房间是否连接周围的房间
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //玩家离开房间后，将房间变得透明
            ChangeRoomTransparency(HiddenTransparency);


            //确保只在场景持续存在且未卸载时，才会使用DoTeen（否则在场景卸载后会报错）
            if (SceneManager.GetActiveScene().isLoaded)
            {
                //将相机亮度一瞬间的变暗
                m_PostProcessController.DarkenThenBrighten(m_DarkPostProcessColorValue, m_PostProcessDuration);
            }
            

            if (m_DoorInsideThisRoom != null)
            {
                m_DoorInsideThisRoom.RoomTrigger.enabled = true;    //玩家离开房间后重新激活门的触发器，从而让玩家之后再进入时生成敌人             
            }             
        }
    }




    //更改房间整体的透明度
    private void ChangeRoomTransparency(float alphaVal)
    {
        foreach(var sprite in m_AllSprites)
        {
            //检查精灵图是否为空，防止报错
            if (sprite != null)
            {
                var tempColor = sprite.color;
                tempColor.a = alphaVal;
                sprite.color = tempColor;
            }          
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
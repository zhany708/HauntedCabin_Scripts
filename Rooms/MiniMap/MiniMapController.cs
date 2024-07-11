using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Introduction：
 * Creator：Zhang Yu
*/

public class MiniMapController : MonoBehaviour
{
    public SpriteRenderer BaseSprite;                           //基础物体的精灵图组件
    public List<GameObject> BackupFrames;                       //储存所有的备用门框


    public static Vector2 CurrentRoomPosPlayerAt;               //玩家当前所在房间的坐标（静态的，表示所有此类共用这一个坐标）

    //用于表示是否进入过初始房间（入口大堂等）
    public Color LightGreen = new Color(250, 250, 250);         //初始房间还未进入时在小地图上显示的颜色
    public Color DarkGreen = new Color(250, 250, 250);          //初始房间进入后在小地图上显示的颜色


    //以下是所有备用门框物体的统一名字
    public const string LeftBackupFrameName = "Left";
    public const string RightBackupFrameName = "Right";
    public const string UpBackupFrameName = "Up";
    public const string DownBackupFrameName = "Down";

    //小地图层级的名字
    public const string MiniMapLayerName = "MiniMap";               


    //用于表示是否进入过基础房间
    public const float InVisibleAlpha = 0.25f;          //玩家没进入时的透明度
    public const float VisibleAlpha = 0.75f;            //玩家进入后的透明度



    






    #region Unity内部函数
    private void Awake()
    {
        if (BaseSprite == null || BackupFrames.Count == 0)
        {
            Debug.LogError("Some components are not assigned correctly in the " + name);
            return;
        }
    }
    #endregion


    #region 主要函数
    //更改基础物体的精灵图的透明度（用于在小地图中表示玩家是否进入过该房间）
    public void ChangeSpriteTransparency(bool isVisible)
    {
        Color tempColor = BaseSprite.color;         //先创建一个临时颜色组件（因为精灵图组件的透明度不可直接更改）

        //随后根据参数更改该临时颜色组件的透明度   
        if (isVisible)
        {
            tempColor.a = VisibleAlpha;
        }

        else
        {
            tempColor.a = InVisibleAlpha;
        }

        BaseSprite.color = tempColor;               //更改完毕后再赋值回精灵图
    }


    //激活某个备用门框物体（当房间的某个门因为一些原因永久关闭后，调用该函数以在小地图上表示该门不可进）
    public void SetActiveBackupFrame(string doorName)
    {
        //根据需要永久关闭的门激活对应的门框物体
        switch (doorName)
        {
            case RoomManager.LeftDoorName:
                FindBackupFrameObject(LeftBackupFrameName).SetActive(true);
                break;

            case RoomManager.RightDoorName:
                FindBackupFrameObject(RightBackupFrameName).SetActive(true);
                break;

            case RoomManager.UpDoorName:
                FindBackupFrameObject(UpBackupFrameName).SetActive(true);
                break;

            case RoomManager.DownDoorName:
                FindBackupFrameObject(DownBackupFrameName).SetActive(true);
                break;

            default:
                Debug.Log("There is something wrong with the parameter!");
                break;
        };
    }

    //在子物体中寻找参数中的名字的物体
    private GameObject FindBackupFrameObject(string targetName)
    {
        foreach (GameObject frame in BackupFrames)
        {
            if (frame.name == targetName)
            {
                return frame;
            }
        }

        Debug.LogError("Cannot find the corresponding BackupFrame: " + targetName);
        return null;
    }



    //获取参数一物体下面所有层级为参数二的精灵图组件（考虑将此函数放进一个管理器内，因为其它脚本可能也需要调用）
    public static List<SpriteRenderer> GetSpriteRenderersWithLayer(GameObject parentObject, string layerName)
    {
        //创建即将返回的列表和参数中层级的序列
        List<SpriteRenderer> resultList = new List<SpriteRenderer>();
        int layer = LayerMask.NameToLayer(layerName);

        if (parentObject == null)
        {
            Debug.LogError("Parent GameObject is null");
            return resultList;
        }

        //获取父物体下所有的精灵图组件
        SpriteRenderer[] allSpriteRenderers = parentObject.GetComponentsInChildren<SpriteRenderer>();

        //随后检索这些组件，将有参数中层级的精灵图加进列表
        foreach (var spriteRenderer in allSpriteRenderers)
        {
            if (spriteRenderer.gameObject.layer == layer)
            {
                resultList.Add(spriteRenderer);
            }
        }

        return resultList;
    }

    #endregion


    #region 检查函数
    //检查当前房间是否需要在小地图中显示（只在小地图中显示玩家方圆内最近的9个房间）
    public static void CheckIfDisplayMiniMap()
    {
        foreach (var roomPos in RoomManager.Instance.GeneratedRoomDict.Keys)
        {
            //检查房间是否在玩家当前房间横向上的一格之外，或者纵向上的一格之外
            if (Mathf.Abs(roomPos.x - CurrentRoomPosPlayerAt.x) > RoomManager.RoomLength ||
                Mathf.Abs(roomPos.y - CurrentRoomPosPlayerAt.y) > RoomManager.RoomWidth)
            {
                //隐藏该房间下的所有层级为小地图的精灵图组件
                foreach (var spriteRenderer in GetSpriteRenderersWithLayer(RoomManager.Instance.GeneratedRoomDict[roomPos], MiniMapLayerName))
                {
                    spriteRenderer.enabled = false;
                }
            }

            //当房间位于玩家方圆最近的9个房间内时
            else
            {
                //激活该房间下的所有层级为小地图的精灵图组件
                foreach (var spriteRenderer in GetSpriteRenderersWithLayer(RoomManager.Instance.GeneratedRoomDict[roomPos], MiniMapLayerName))
                {
                    spriteRenderer.enabled = true;
                }
            }
        }
    }
    #endregion


    #region 其余函数
    public void ResetGame()
    {
        //更改所有房间该模块的精灵图透明度，以表示玩家还没进入该房间
        ChangeSpriteTransparency(false);
    }
    #endregion
}
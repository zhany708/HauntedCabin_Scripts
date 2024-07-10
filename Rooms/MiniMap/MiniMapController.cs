using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Introduction：
 * Creator：Zhang Yu
*/

public class MiniMapController : MonoBehaviour
{
    public SpriteRenderer BaseSprite;                   //基础物体的精灵图组件
    public List<GameObject> BackupFrames;               //储存所有的备用门框


    //以下是所有备用门框物体的统一名字
    public const string LeftBackupFrameName = "Left";
    public const string RightBackupFrameName = "Right";
    public const string UpBackupFrameName = "Up";
    public const string DownBackupFrameName = "Down";


    public const float InVisibleAlpha = 0.25f;          //玩家没进入时的透明度
    public const float VisibleAlpha = 0.75f;            //玩家进入后的透明度





    private void Awake()
    {
        if (BaseSprite == null || BackupFrames.Count == 0)
        {
            Debug.LogError("Some components are not assigned correctly in the " + name);
            return;
        }
    }



    //更改基础物体的精灵图的透明度（用于在小地图中表示玩家是否进入过该房间）
    public void ChangeSpriteTransparency(bool isVisible)
    {
        Color tempColor = BaseSprite.color;         //先创建一个临时颜色组件

        if (isVisible)
        {          
            tempColor.a = VisibleAlpha;             //随后更改该临时颜色组件的透明度            
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
        //根据参数中的房间名，激活对应的备用门框
        switch (doorName)
        {
            case RoomManager.LeftDoorName:

                break;

            case RoomManager.RightDoorName:

                break;

            case RoomManager.UpDoorName:

                break;

            case RoomManager.DownDoorName:

                break;

            default:
                Debug.Log("");
                break;
        };
    }
}

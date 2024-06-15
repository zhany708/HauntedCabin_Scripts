using UnityEngine;


public class DownDoorTrigger : SideDoorController        //用于下侧门的子物体
{


    protected override void Awake()
    {
        sprite = GetComponentInParent<SpriteRenderer>();     //物体本身只有碰撞框，因此精灵图需要从父物体那获取
    }
}
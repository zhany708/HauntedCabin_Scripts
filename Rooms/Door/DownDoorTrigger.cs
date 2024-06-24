using UnityEngine;



//用于下侧门的子物体（因为下侧门如果不将碰撞框与物体分离的的话就会出现子弹无法穿过门的情况）
public class DownDoorTrigger : SideDoorController
{


    protected override void Awake()
    {
        sprite = GetComponentInParent<SpriteRenderer>();     //物体本身只有碰撞框，因此精灵图需要从父物体那获取
    }
}
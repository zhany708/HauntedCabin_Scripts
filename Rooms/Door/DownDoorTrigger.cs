using UnityEngine;


public class DownDoorTrigger : SideDoorController        //用于下侧门的子物体
{


    protected override void Awake()
    {
        sprite = GetComponentInParent<SpriteRenderer>();
    }
}
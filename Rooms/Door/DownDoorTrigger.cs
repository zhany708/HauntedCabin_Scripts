using UnityEngine;


public class DownDoorTrigger : SideDoorController        //�����²��ŵ�������
{


    protected override void Awake()
    {
        sprite = GetComponentInParent<SpriteRenderer>();
    }
}
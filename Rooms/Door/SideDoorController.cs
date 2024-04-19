using System.Collections.Generic;
using UnityEngine;


public class SideDoorController : MonoBehaviour
{
    public float XOffset;
    public float YOffset;

    //透明度的值
    public float TransparentValue = 0f;



    protected SpriteRenderer sprite;


    List<Collider2D> m_AllObjects;





    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();     
    }

    private void Start()
    {
        //初始化
        m_AllObjects = new List<Collider2D>();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        //只有玩家或敌人触发了门的触发器后，才会降低透明度
        if (sprite != null && ( other.CompareTag("Player") || other.CompareTag("Enemy")) )
        {
            m_AllObjects.Add(other);

            //降低门的透明度
            sprite.color = new Color(1f, 1f, 1f, TransparentValue);
        }


        /*
        if (other.CompareTag("Player"))
        {
            
            Player player = other.GetComponentInParent<Player>();

            Vector2 movingDirection = player.InputHandler.RawMovementInput;
            Vector2 teleportPos;    //传送坐标


            //根据角色移动方向进行瞬移，这样就不用确定当前门的位置
            if (movingDirection.x != 0)
            {
                if (movingDirection.x > 0)
                {
                    teleportPos = new Vector2(transform.position.x + XOffset, transform.position.y - 0.5f);  //由于角色坐标位于脚步，所以Y轴上也要偏移一点
                }
                else
                {
                    teleportPos = new Vector2(transform.position.x - XOffset, transform.position.y - 0.5f);
                }
            }


            else
            {
                if (movingDirection.y > 0)
                {
                    teleportPos = new Vector2(transform.position.x, transform.position.y + YOffset);
                }
                else
                {
                    teleportPos = new Vector2(transform.position.x, transform.position.y - YOffset);
                }
            }
    
            player.gameObject.transform.position = teleportPos;     //传送玩家
                 
        }
        */
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        //先检查离开碰撞器的是否为玩家或敌人
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            //在检查列表的元素数量是否为0
            if (m_AllObjects.Count > 0)
            {
                m_AllObjects.Remove(other);
            }

            //只要仍然有触发器没有离开门，那么即使玩家/敌人离开了门的触发器，门依然保持半透明
            if (m_AllObjects.Count == 0 && sprite.color.a == TransparentValue)     //当玩家离开门后，且门的透明度被更改过
            {
                if (sprite != null)
                {
                    //调回门的透明度
                    sprite.color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
    }
}
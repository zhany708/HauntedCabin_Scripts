using System.Collections.Generic;
using UnityEngine;


public class SideDoorController : MonoBehaviour
{
    //角色传送时需要的数值，用于表示将要传送的房间坐标的偏移
    public float XOffset = 17f;
    public float YOffset = 10.7f;

    //房间隐藏时透明度的值
    public float HiddenTransparency = 0.5f;



    protected SpriteRenderer sprite;

    //用于储存所有触发了门的碰撞器
    List<Collider2D> m_AllObjects;

    const float m_DefaultTransparency = 1f;






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
        if (other.CompareTag("Player") || other.CompareTag("Enemy") )
        {
            m_AllObjects.Add(other);

            //降低门的透明度
            ChangeTransparency(HiddenTransparency);
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
            //再检查列表的元素数量是否为0
            if (m_AllObjects.Count > 0)
            {
                m_AllObjects.Remove(other);
            }

            //只要仍然有触发器没有离开门，那么即使玩家/敌人离开了门的触发器，门依然保持半透明
            if (m_AllObjects.Count == 0 && sprite.color.a == HiddenTransparency)        //只有门的透明度为此脚本中的变量时，才调回透明度
            {
                ChangeTransparency(m_DefaultTransparency);
            }
        }
    }




    private void ChangeTransparency(float alphaVal)
    {
        if (sprite != null)
        {
            //更改门的透明度
            sprite.color = new Color(1f, 1f, 1f, alphaVal);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDoorController : MonoBehaviour
{
    public float XOffset;
    public float YOffset;

    public float TransparentValue = 0f;

    SpriteRenderer m_Sprite;
    


    private void Awake()
    {
        m_Sprite = GetComponent<SpriteRenderer>();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            /*
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
            */

            
            if (m_Sprite != null)
            {
                //降低门的透明度
                m_Sprite.color = new Color(1f, 1f, 1f, TransparentValue);
            }       
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && m_Sprite.color.a == TransparentValue)     //当玩家离开门后，且门的透明度被更改过
        {          
            if (m_Sprite != null)
            {
                //调回门的透明度
                m_Sprite.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
}

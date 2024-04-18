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
            Vector2 teleportPos;    //��������


            //���ݽ�ɫ�ƶ��������˲�ƣ������Ͳ���ȷ����ǰ�ŵ�λ��
            if (movingDirection.x != 0)
            {
                if (movingDirection.x > 0)
                {
                    teleportPos = new Vector2(transform.position.x + XOffset, transform.position.y - 0.5f);  //���ڽ�ɫ����λ�ڽŲ�������Y����ҲҪƫ��һ��
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
    
            player.gameObject.transform.position = teleportPos;     //�������
            */

            
            if (m_Sprite != null)
            {
                //�����ŵ�͸����
                m_Sprite.color = new Color(1f, 1f, 1f, TransparentValue);
            }       
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && m_Sprite.color.a == TransparentValue)     //������뿪�ź����ŵ�͸���ȱ����Ĺ�
        {          
            if (m_Sprite != null)
            {
                //�����ŵ�͸����
                m_Sprite.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
}

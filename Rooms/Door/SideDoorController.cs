using System.Collections.Generic;
using UnityEngine;


public class SideDoorController : MonoBehaviour
{
    public float XOffset;
    public float YOffset;

    //͸���ȵ�ֵ
    public float TransparentValue = 0f;



    protected SpriteRenderer sprite;


    List<Collider2D> m_AllObjects;





    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();     
    }

    private void Start()
    {
        //��ʼ��
        m_AllObjects = new List<Collider2D>();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        //ֻ����һ���˴������ŵĴ������󣬲Żή��͸����
        if (sprite != null && ( other.CompareTag("Player") || other.CompareTag("Enemy")) )
        {
            m_AllObjects.Add(other);

            //�����ŵ�͸����
            sprite.color = new Color(1f, 1f, 1f, TransparentValue);
        }


        /*
        if (other.CompareTag("Player"))
        {
            
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
                 
        }
        */
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        //�ȼ���뿪��ײ�����Ƿ�Ϊ��һ����
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            //�ڼ���б��Ԫ�������Ƿ�Ϊ0
            if (m_AllObjects.Count > 0)
            {
                m_AllObjects.Remove(other);
            }

            //ֻҪ��Ȼ�д�����û���뿪�ţ���ô��ʹ���/�����뿪���ŵĴ�����������Ȼ���ְ�͸��
            if (m_AllObjects.Count == 0 && sprite.color.a == TransparentValue)     //������뿪�ź����ŵ�͸���ȱ����Ĺ�
            {
                if (sprite != null)
                {
                    //�����ŵ�͸����
                    sprite.color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
    }
}
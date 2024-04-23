using UnityEngine;


//���ڴ��ͽ�ɫ�Ľű������ڲ��ŵ���������
public class TeleportController : MonoBehaviour
{
    public enum DoorType { Up, Down, Left, Right }
    public DoorType doorType;


    public float LeftAndRightOffset = 1.5f;
    public float UpOffset = 1.5f;
    public float DownOffset = 4.5f;







    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //��ȡ������
            Player player = other.GetComponentInParent<Player>();

            if (player != null)
            {
                Vector2 teleportPos = CalculateTeleportPosition(player.transform.position);
                player.transform.position = teleportPos;
            }

        }
    }


    //�����ŵ����ͼ����ɫ��˲�ƾ���
    private Vector2 CalculateTeleportPosition(Vector2 playerPosition)
    {
        switch (doorType)
        {
            case DoorType.Up:
                return new Vector2(playerPosition.x, transform.position.y + UpOffset);      //���ֽ�ɫ�����X��ǰ�󲻱�

            case DoorType.Down:
                return new Vector2(playerPosition.x, transform.position.y - DownOffset);

            case DoorType.Left:
                return new Vector2(transform.position.x - LeftAndRightOffset, playerPosition.y);        //���ֽ�ɫ�����Y��ǰ�󲻱�

            case DoorType.Right:
                return new Vector2(transform.position.x + LeftAndRightOffset, playerPosition.y);

            default:
                return playerPosition;  //Ĭ�Ϸ���ԭʼλ�ã���ֹ����
        }
    }

}
using UnityEngine;


//用于传送角色的脚本，放在侧门的子物体上
public class TeleportController : MonoBehaviour
{
    public enum DoorType { Up, Down, Left, Right }
    public DoorType doorType;


    public float LeftAndRightOffset = 1.5f;
    public float UpOffset = 1.5f;
    public float DownOffset = 4.5f;






    #region Unity内部函数
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //获取玩家组件
            Player player = other.GetComponentInParent<Player>();
            if (player == null)
            {
                Debug.LogError("Player component not found in the parent gameObject.");
                return;     
            }

            Vector2 teleportPos = CalculateTeleportPosition(player.transform.position);
            player.transform.position = teleportPos;
        }
    }
    #endregion


    #region 主要函数
    //根据门的类型计算角色的瞬移距离
    private Vector2 CalculateTeleportPosition(Vector2 playerPosition)
    {
        switch (doorType)
        {
            case DoorType.Up:
                return new Vector2(playerPosition.x, transform.position.y + UpOffset);      //保持角色坐标的X轴前后不变

            case DoorType.Down:
                return new Vector2(playerPosition.x, transform.position.y - DownOffset);

            case DoorType.Left:
                return new Vector2(transform.position.x - LeftAndRightOffset, playerPosition.y);        //保持角色坐标的Y轴前后不变

            case DoorType.Right:
                return new Vector2(transform.position.x + LeftAndRightOffset, playerPosition.y);

            default:
                return playerPosition;  //默认返回原始位置，防止报错
        }
    }
    #endregion
}
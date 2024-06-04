using UnityEngine;



public class NewBehaviourScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //只有当玩家身上没有护符时才会触发效果
        if (other.gameObject.CompareTag("Player") && !HellsCall.Instance.GetCanStartRitual() )
        {
            //调整剧本物体中的布尔参数，表示玩家可以进行仪式
            HellsCall.Instance.SetCanStartRitual(true);

            Destroy(gameObject);        //删除祷告石护符物体
        }
    }
}

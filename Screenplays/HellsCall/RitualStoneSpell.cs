using UnityEngine;



public class RitualStoneSpell : MonoBehaviour       //祷告石护符
{
    #region Unity内部函数
    private void Start()
    {
        //在这里加进字典，防止字典还没实例化就尝试获取引用导致报错
        if (!HellsCall.Instance.AllStonePosDict.ContainsKey(transform.position))
        {
            HellsCall.Instance.AllStonePosDict.Add(transform.position, gameObject);
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        //只有当玩家身上没有护符时才会触发效果
        if (other.gameObject.CompareTag("Player") && !HellsCall.Instance.GetCanStartRitual() )
        {
            UIManager.Instance.OpenInteractPanel(() => PickupLogic());     //打开互动面板
        }
    }
   
    private void OnTriggerExit2D(Collider2D other)
    {
        //检查是否是玩家碰撞，再检查界面是否存于字典中，最后再检查界面是否打开
        if (other.CompareTag("Player") && UIManager.Instance.PanelDict.ContainsKey(UIManager.Instance.UIKeys.InteractPanel)
            && !InteractPanel.Instance.IsRemoved)
        {
            UIManager.Instance.ClosePanel(UIManager.Instance.UIKeys.InteractPanel, true);      //淡出互动界面
        }
    }

    private void OnDestroy() 
    {
        //从地狱的呼唤剧本脚本的字典中删除坐标
        if (HellsCall.Instance.AllStonePosDict.ContainsKey(transform.position))
        {
            HellsCall.Instance.AllStonePosDict.Remove(transform.position);
        }
    }
    #endregion


    #region 主要函数
    private void PickupLogic()      //玩家拾取护符时的逻辑
    {
        //调整剧本物体中的布尔参数，表示玩家可以进行仪式
        HellsCall.Instance.SetCanStartRitual(true);

        Destroy(gameObject);        //删除祷告石护符物体
    }
    #endregion
}
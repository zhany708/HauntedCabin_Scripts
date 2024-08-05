using UnityEngine;



public class RitualStoneSpell : MonoBehaviour       //祷告石护符
{
    public string InteractTextPhraseKey;            //传递给互动界面的文本
    public string TipTextPhraseKey;                 //传递给提示界面的文本






    #region Unity内部函数
    private void Awake()
    {
        if (InteractTextPhraseKey == "" || TipTextPhraseKey == "")
        {
            Debug.LogError("Some components are not assigned in the " + gameObject.name);
            return;
        }
    }

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
        if (other.gameObject.CompareTag("Player") )
        {
            //只有当玩家身上没有护符时才会触发效果
            if (!HellsCall.Instance.GetCanStartRitual() )
            {
                UIManager.Instance.OpenInteractPanel(() => PickupLogic(), InteractTextPhraseKey);     //打开互动面板
            }

            //需要做的：当玩家身上已经有护符时提醒玩家      
            else
            {
                //先赋值文本，再打开提示面板
                TipPanel.Instance.UpdatePanelText(TipTextPhraseKey);
                TipPanel.Instance.OpenPanel();
            }
        }
    }
   
    private void OnTriggerExit2D(Collider2D other)
    {
        //检查是否是玩家碰撞
        if (other.CompareTag("Player") )
        {
            //检查互动界面是否存于字典中，最后再检查互动界面是否打开
            if (UIManager.Instance.PanelDict.ContainsKey(UIManager.Instance.UIKeys.InteractPanel)
                && !InteractPanel.Instance.IsRemoved)
            {
                UIManager.Instance.ClosePanel(UIManager.Instance.UIKeys.InteractPanel, true);       //淡出互动界面
            }


            if (UIManager.Instance.PanelDict.ContainsKey(UIManager.Instance.UIKeys.TipPanel)
                && !TipPanel.Instance.IsRemoved)
            {
                UIManager.Instance.ClosePanel(UIManager.Instance.UIKeys.TipPanel, true);            //淡出提示界面
            }
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
using UnityEngine;

/*
 * Introduction：
 * Creator：Zhang Yu
*/

public class ResultFlow : MonoBehaviour
{
    SlotMachinePanel m_SlotMachinePanel;








    #region Unity内部函数
    private void Awake()
    {
        m_SlotMachinePanel = GetComponentInParent<SlotMachinePanel>();
        if (m_SlotMachinePanel == null)
        {
            Debug.LogError($"Cannot get the SlotMachinePanel reference from the parent of {gameObject.name}");
            return;
        }
    }
    #endregion


    #region 动画帧事件
    private void HandleFlowAnimationFinished()
    {
        m_SlotMachinePanel.GetScreenplayBackground().gameObject.SetActive(true);              //激活剧本界面

        m_SlotMachinePanel.StartTipTextAnimation();         //激活提示文本，以让玩家继续游戏         
    }
    #endregion
}
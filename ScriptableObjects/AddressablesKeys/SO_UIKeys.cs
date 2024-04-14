using UnityEngine;


[CreateAssetMenu(fileName = "newUIKeys", menuName = "Data/UI Data/UI Keys")]
public class SO_UIKeys : ScriptableObject
{
    public string PlayerStatusBarKey;    //玩家状态栏。在HealthBar脚本里初始化
    public string TransitionStagePanelKey;    //进入二阶段文字
}
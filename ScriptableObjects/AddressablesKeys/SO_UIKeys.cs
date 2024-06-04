using UnityEngine;


[CreateAssetMenu(fileName = "newUIKeys", menuName = "Data/UI Data/UI Keys")]
public class SO_UIKeys : ScriptableObject
{
    //菜单相关
    [Header("Menu")]
    public string MainMenuPanel;        //游戏开始菜单
    public string SettingPanel;         //游戏设置界面
    public string PauseMenuPanel;       //游戏暂停界面
    public string GameLostPanel;        //游戏失败界面
    public string GameWinningPanel;     //游戏胜利界面

    public string TaskPanel;                //游戏任务界面


    //剧本相关
    [Header("ScreenPlay")]
    public string GameBackgroundPanel;      //游戏底层剧本界面
    public string HellsCallPanel;           //地域的呼唤剧本背景界面


    //人物相关
    [Header("Player")]
    public string PlayerStatusBarKey;         //玩家状态栏。在HealthBar脚本里初始化


    //事件相关
    [Header("Event")]
    public string TransitionStagePanelKey;    //进入二阶段文字
    public string EvilTelephonePanel;         //《电话铃声》事件界面


    //武器相关
    [Header("Weapon")]
    public string PickupWeaponPanelKey;       //用于玩家拾取武器  
}
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
    public string GameWinningPanel;     //游戏胜利界面（目前不需要）

    public string TaskPanel;            //游戏任务界面
    public string MiniMapPanel;         //小地图


    //剧本相关
    [Header("ScreenPlay")]
    public string GameBackgroundPanel;              //游戏底层剧本界面
    public string HellsCallBackground;              //地狱的呼唤剧本背景界面
    public string HellsCall_GameLostPanel;          //地狱的呼唤剧本失败界面
    public string HellsCall_GameWinningPanel;       //地狱的呼唤剧本胜利界面
    public string HellsCall_AltarHealthBarPanel;    //地狱的呼唤剧本里祷告石的血条


    //人物相关
    [Header("Player")]
    public string PlayerStatusBarKey;         //玩家状态栏。在HealthBar脚本里初始化


    //事件相关
    [Header("Event")]
    public string TransitionStagePanelKey;    //进入二阶段文字（目前不需要）
    public string EvilTelephonePanel;         //《电话铃声》事件界面


    //武器相关
    [Header("Weapon")]
    public string PickupWeaponPanelKey;       //用于玩家拾取武器  


    //其他
    [Header("Other")]
    public string ConfirmPanel;         //确认界面
    public string InteractPanel;        //交互界面
    public string TipPanel;             //提示界面
    public string RoomNamePanel;        //显示房间名的界面
    public string SlotMachinePanel;     //老虎机界面
    public string QTEPanel;             //QTE检测界面
    public string QTEPanelWithMoreZones;             //QTE检测界面
}
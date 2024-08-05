using System.Collections;
using UnityEngine;
using ZhangYu.Utilities;




public class Altar : MonoBehaviour      //仪式台的脚本
{
    public GameObject EnemyPrefab;      //敌人的预制件（用于在仪式期间生成）
    public string InteractTextPhraseKey;            //传递给互动界面的文本
    public string TipTextPhraseKey;                 //传递给提示界面的文本


    public Core Core { get; private set; }

    
    //检查m_Combat是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    public Combat Combat => m_Combat ? m_Combat : Core.GetCoreComponent(ref m_Combat);
    private Combat m_Combat;
    

    public Stats Stats => m_Stats ? m_Stats : Core.GetCoreComponent(ref m_Stats);
    private Stats m_Stats;



    Coroutine m_EnemySpawnCoroutine;    //敌人生成的协程

    Timer m_DurationTimer;              //用于计时仪式时长的计时器

    [SerializeField] float m_RitualMaxHealth = 150f;                //仪式台的生命值上限
    [SerializeField] float m_HitResistance = 99f;                   //仪式台的受击退抗性

    [SerializeField] float m_RitualDuration = 30f;                  //仪式时间
    [SerializeField] float m_EnemySpawnInterval = 3.5f;             //敌人生成的冷却
    [SerializeField] float m_RestorePlayerHealthAmout = 15f;        //完成仪式后玩家恢复的生命值
    [SerializeField] float m_RestoreAltarHealthPercent = 0.3f;      //完成仪式后仪式台恢复的生命值比例







    #region Unity内部函数
    private void Awake()
    {
        //初始化Core组件
        Core = GetComponentInChildren<Core>();      //从子物体那调用Core脚本
        Core.SetParameters(m_RitualMaxHealth, 0, m_HitResistance);   //将参数传给Core

        //初始化计数器
        m_DurationTimer = new Timer(m_RitualDuration);


        if (EnemyPrefab == null || InteractTextPhraseKey == "" || TipTextPhraseKey == "")
        {
            Debug.LogError("One or more components are not assigned on " + gameObject.name);
            return;
        }
    }

    private void OnEnable()
    {
        m_DurationTimer.OnTimerDone += FinishRitual;          //计时结束后进行仪式结束的逻辑
        Stats.OnHealthZero += GameLost;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {       
        if (other.gameObject.CompareTag("Player") )
        {
            //只有当玩家拿到祷告石后，才允许玩家开始仪式
            if (HellsCall.Instance.GetCanStartRitual())
            {
                UIManager.Instance.OpenInteractPanel(() => SetAnimatorStart(), InteractTextPhraseKey);     //打开互动面板    
            }

            //需要做的：当玩家身上没有护符时，提醒玩家需要获得某些物品以开始仪式
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

    private void OnDisable()
    {
        m_DurationTimer.OnTimerDone -= FinishRitual;
        Stats.OnHealthZero -= GameLost;
    }
    #endregion


    #region 仪式相关
    //设置动画器参数，以开始仪式
    private void SetAnimatorStart()      //先调用这个函数，随后才会在动画中调用开始仪式的函数
    {
        Core.Animator.SetBool("RitualStart", true);    //播放符号发光动画
    }

    private void GenerateEnemyThroughTime(float duration, float spawnInterval)      //仪式期间持续生成敌人
    {
        StartCoroutine(m_DurationTimer.WaitForDuration() );     //开始仪式计时

        //开始生成敌人
        m_EnemySpawnCoroutine = StartCoroutine(EnemySpawnCoroutine(m_EnemySpawnInterval) );
    }

    private IEnumerator EnemySpawnCoroutine(float spawnInterval)        //敌人生成的协程
    {
        while (true)            //反复的生成敌人
        {
            GenerateSingleEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    private void GenerateSingleEnemy()      //生成单个敌人
    {
        //在房间内的随机位置生成敌人
        EnvironmentManager.Instance.GenerateEnemy(HellsCall.Instance.RitualRoomDoorController, EnemyPrefab);
    }



    private void FinishRitual()   //仪式结束后的逻辑
    {
        Core.Animator.SetBool("RitualStart", false);      //将参数设置为false，以结束仪式台的环绕

        //停止敌人生成的循环
        if (m_EnemySpawnCoroutine != null)
        {
            StopCoroutine(m_EnemySpawnCoroutine);
        }

        if (!EnvironmentManager.Instance.IsGameLost)      //只有在游戏没有失败的时候才会进行下面的逻辑
        {
            //检查是否有“生成提醒”物体存在，如果有的话则删除
            SpawnWarning sapwnWarningObject = ParticlePool.Instance.gameObject.GetComponentInChildren<SpawnWarning>();
            if (sapwnWarningObject != null)
            {
                Destroy(sapwnWarningObject.gameObject);
            }


            Stats.IncreaseHealth(m_RitualMaxHealth * m_RestoreAltarHealthPercent);          //给祷告石回一点血

            EnemyPool.Instance.KillAllEnemy_DefenseWar();                                   //立刻消灭所有敌人

            RitualRoom.Instance.DoorControllerInsideThisRoom.SetIsDoorOpenable(true);       //设置布尔，表示当前房间门可以打开

            //检查房间原来生成的敌人（非仪式生成的）是否清理干净
            if (RitualRoom.Instance.DoorControllerInsideThisRoom.IsAllEnemyKilled)
            {
                RitualRoom.Instance.DoorControllerInsideThisRoom.OpenDoors();               //打开仪式房的门
            }
            
            HellsCall.Instance.PlayerStats.IncreaseHealth(m_RestorePlayerHealthAmout);      //给玩家增加一定的血量
            HellsCall.Instance.IncrementRitualCount();                                      //增加仪式完成的计数
        }     
    }


    //跟Stats状态函数里的事件绑定在一起，或者放在仪式台死亡动画里（因为需要跟Event绑定，所以这里的返回类型不能为Task）
    private async void GameLost()       
    {
        //停止敌人生成的循环
        if (m_EnemySpawnCoroutine != null)
        {
            StopCoroutine(m_EnemySpawnCoroutine);
        }

        await HellsCall.Instance.Lose();                     //进行剧本的失败逻辑
    }
    #endregion


    #region 动画帧事件
    private void StartRitual()      //放在仪式台发亮的那一帧
    {
        HellsCall.Instance.SetCanStartRitual(false);                                //仪式开始后将布尔设置为false，防止玩家反复开始仪式
        RitualRoom.Instance.DoorControllerInsideThisRoom.SetIsDoorOpenable(false);  //设置布尔，表示当前房间门无法打开
        RitualRoom.Instance.DoorControllerInsideThisRoom.CloseDoors();              //仪式开始后关闭房间的门

        //在一定时间内，定期在房间内生成敌人（生成位置随机）
        GenerateEnemyThroughTime(m_RitualDuration, m_EnemySpawnInterval);
    }
    #endregion


    #region Getters
    public float GetMaxHealth()
    {
        return m_RitualMaxHealth;
    }
    #endregion
}
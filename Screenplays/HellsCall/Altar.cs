using UnityEngine;
using ZhangYu.Utilities;



public class Altar : MonoBehaviour     //放在仪式台上的脚本
{
    public GameObject EnemyPrefab;      //敌人的预制件


    Timer m_DurationTimer;      //用于计时仪式时长的计时器
    Timer m_EnemySpawnTimer;    //用于持续生成敌人的计时器


    float m_RitualDuration = 60f;        //仪式时间
    float m_EnemySpawnInterval = 3f;    //敌人生成的冷却




    private void Awake()
    {
        m_DurationTimer = new Timer(m_RitualDuration);
        m_EnemySpawnTimer = new Timer(m_EnemySpawnInterval);
    }

    private void Update()
    {
        m_EnemySpawnTimer.Tick();       //持续进行生成敌人的计时
    }

    private void OnEnable()
    {
        m_EnemySpawnTimer.OnTimerDone += GenerateEnemy;     //将函数绑定到计时结束的事件，这样计时结束后就会立刻生成敌人，并再次计时
    }

    private void OnDisable()
    {
        m_EnemySpawnTimer.OnTimerDone -= GenerateEnemy;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //需要做的：在一定时间内，定期在房间内生成敌人（生成位置随机）。敌人只会攻击仪式台，即使受到了玩家攻击（敌人相关的逻辑放在敌人脚本里）
            GenerateEnemyThroughTime(m_RitualDuration, m_EnemySpawnInterval);
        }
    }




    private void GenerateEnemyThroughTime(float duration, float spawnInterval)
    {
        StartCoroutine(m_DurationTimer.WaitForDuration() );     //开始仪式计时

        GenerateEnemy();        //生成第一个敌人

        while (!m_DurationTimer.GetIsTimerDone() )      //仪式期间持续进行的逻辑（不能放只进行一次的函数！）
        {
            
        }

        m_EnemySpawnTimer.StopTimer();      //仪式结束后暂停生成敌人的计时，防止仪式结束后依然持续生成敌人
    }

    private void GenerateEnemy()
    {
        //在房间内的随机位置生成敌人
        EnvironmentManager.Instance.GenerateEnemy(HellsCall.Instance.RitualRoomDoorController, EnemyPrefab);

        m_EnemySpawnTimer.StartTimer();     //生成完敌人后开始计时
    }
}
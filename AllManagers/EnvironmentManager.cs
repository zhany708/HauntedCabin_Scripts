using System;
using UnityEngine;


public class EnvironmentManager : ManagerTemplate<EnvironmentManager>     //用于处理游戏过程中的一些动态变化（比如在某个地方生成某个新东西）
{
    public event Action OnEnemyKilled;       //接收方为TaskPanel


    public GameObject BlockDoorBarrel;      //挡住玩家进入门的障碍物

    public int KilledEnemyCount { get; private set; } = 0;   //表示杀死过多少敌人
    public int RequiredEnemyCount { get; private set; } = 1;        //表示需要杀死多少敌人，游戏才胜利
    public bool IsGameOver { get; private set; } = false;



    







    protected override void Awake()
    {
        base.Awake();

        if (BlockDoorBarrel == null)
        {
            Debug.LogError("BlockDoorBarrel is not assigned in the EnvironmentManager.");
            return;
        }
    }





    //生成木桶，用于阻止玩家穿过门（将这种游戏中的动态变化放进一个Manager中，从而让其他脚本专注于其他的事）
    public void GenerateBarrelToBlockDoor(Transform parentTransform, Vector2 generatedPos)
    {
        Instantiate(BlockDoorBarrel, generatedPos, Quaternion.identity, parentTransform);

        //尝试从父物体那里获取脚本组件
        RootRoomController parentObject = parentTransform.GetComponent<RootRoomController>();

        if (parentObject != null)
        {
            //添加新的精灵图
            parentObject.AddNewSpriteRenderers();
        }
    }


    public async void IncrementKilledEnemyCount()
    {
        KilledEnemyCount++;

        OnEnemyKilled?.Invoke();        //调用回调函数

        if (KilledEnemyCount >= RequiredEnemyCount)     //检查是否杀死了足够的敌人
        {
            //Debug.Log("You win!");      //需要做的：打开游戏胜利界面，将敌人对象池中的所有敌人强行进入死亡状态
            IsGameOver = true;

            EnemyPool.Instance.EndGame();       //在场景中让所有激活的敌人进入死亡状态

            await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.GameWinningPanel);
        }
    }
}
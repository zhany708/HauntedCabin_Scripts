using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



//用于处理游戏过程中的一些动态变化（比如在某个地方生成某个新东西）
public class EnvironmentManager : ManagerTemplate<EnvironmentManager>     
{
    public GameObject SpawnWarningObject;


    public bool IsGameLost { get; private set; } = false;
    public bool IsFirstTimeEnterGame { get; private set; } = true;




    #region 生成物体相关
    //生成物体，同时将参数中的Transform设置为物体的父物体
    public void GenerateObjectWithParent(GameObject generatedObject, Transform parentTransform, Vector2 generatedPos)
    {
        Instantiate(generatedObject, generatedPos, Quaternion.identity, parentTransform);

        //生成完物体后立刻检查是否应该在小地图中显示出来
        MiniMapController.CheckIfDisplayMiniMap();
    }


    //生成警告物体（通常用于生成敌人/事件前），同时传递物体动画播放后需要进行的逻辑
    public void GenerateSpawnWarningObject(Action relatedAction, Vector2 objectPos)
    {
        //从敌人对象池中生成警告物体
        GameObject spawnWarningObject = ParticlePool.Instance.GetObject(SpawnWarningObject, objectPos);

        //从警告物体那获取警告脚本
        SpawnWarning spawnWarning = spawnWarningObject.GetComponent<SpawnWarning>();
        if (spawnWarning == null)
        {
            Debug.LogError("Cannot get the SpawnWarning component in the: " + spawnWarning.name);
            return;
        }


        spawnWarning.OnAnimationFinished += relatedAction;      //将参数中的函数绑定到事件
    }
    #endregion


    #region 生成敌人相关
    //在房间内随机的生成单个敌人
    public void GenerateEnemy(DoorController doorController, GameObject enemyPrefab)
    {
        Vector2 spawnPos = doorController.EnemySpwanPos.GenerateSingleRandomPos();  //生成随机坐标

        EnsureNoFurnitureCollision(ref spawnPos, doorController);      //检查是否跟家具重叠


        //这里的enemy物体是敌人的跟物体（包含巡逻坐标的），在生成的同时赋予物体生成坐标
        GameObject enemyObject = EnemyPool.Instance.GetObject(enemyPrefab, spawnPos);     //从敌人对象池中生成敌人

        InitializeEnemy(enemyObject, doorController);
    }

    /*
    //根据房间提前设置的敌人数量生成敌人
    public void GenerateEnemy(DoorController doorController)
    {
        if (doorController.EnemyObjects.Length != 0)   //如果房间有怪物
        {
            List<Vector2> enemySpawnList = doorController.EnemySpwanPos.GenerateMultiRandomPos(doorController.EnemyObjects.Length);     //根据怪物数量生成随机坐标list

            //生成完坐标列表后。检查列表中是否有跟家具重合的坐标
            EnsureNoFurnitureCollision(enemySpawnList, doorController);


            for (int i = 0; i < doorController.EnemyObjects.Length; i++)
            {
                
                //从敌人对象池中生成警告物体
                GameObject spawnWarningObject = EnemyPool.Instance.GetObject(SpawnWarningObject, enemySpawnList[i]);     
               
                //从警告物体那获取警告脚本
                SpawnWarning spawnWarning = spawnWarningObject.GetComponent<SpawnWarning>();
                if (spawnWarning == null)
                {
                    Debug.LogError("Cannot get the SpawnWarning component in the: " + spawnWarning.name);
                    return;
                }

                spawnWarning.SetEnemyObject(doorController.EnemyObjects[i]);
                spawnWarning.SetDoorController(doorController);
                



                
                //生成警告物体，同时将对应的逻辑传递给物体
                GenerateSpawnWarningObject(() => LogicPassToSpawnWarningObject(doorController.EnemyObjects[i], enemySpawnList[i], doorController)
                , enemySpawnList[i]);
                

                Debug.Log("The coreesponding GameObject at index " + i + " is: " + doorController.EnemyObjects[i].name);

                GenerateSpawnWarningObject(() => LogicPassToSpawnWarningObject(doorController.EnemyObjects[i], enemySpawnList[i], doorController)
                , enemySpawnList[i]);



                
                //这里的enemy物体是敌人的跟物体（包含巡逻坐标的），在生成的同时赋予物体生成坐标
                GameObject enemyObject = EnemyPool.Instance.GetObject(doorController.EnemyObjects[i], enemySpawnList[i]);     //从敌人对象池中生成敌人

                //Debug.Log("The enemy spawn position is : " + enemySpawnList[i]);

                InitializeEnemy(enemyObject, doorController);
                
            }
        }
    }
    */

    public void GenerateEnemy(DoorController doorController)
    {
        //检查门控制器是否为空，或是否有敌人
        if (doorController.EnemyObjects == null || doorController.EnemyObjects.Length == 0)
        {
            Debug.LogError("EnemyObjects array in the DoorController is null or empty.");
            return;
        }

        //创建随机坐标，并放进列表
        List<Vector2> enemySpawnList = doorController.EnemySpwanPos.GenerateMultiRandomPos(doorController.EnemyObjects.Length);

        //生成完坐标后检查列表是否配置正确
        if (enemySpawnList == null || enemySpawnList.Count != doorController.EnemyObjects.Length)
        {
            Debug.LogError("Enemy spawn list was not generated correctly. Check the GenerateMultiRandomPos method.");
            return;
        }


        //确保生成的坐标不与家具重叠
        EnsureNoFurnitureCollision(enemySpawnList, doorController);


        for (int i = 0; i < doorController.EnemyObjects.Length; i++)
        {
            //检查索引是否超出列表
            if (i >= enemySpawnList.Count)
            {
                Debug.LogError($"Index out of range: enemySpawnList count is {enemySpawnList.Count} but trying to access index {i}");
                continue;
            }

            //检查要获取的敌人物体是否为空
            if (doorController.EnemyObjects[i] == null)
            {
                Debug.LogError($"doorController.EnemyObjects[{i}] is null.");
                continue;
            }

            //创建临时变量，以储存列表中的变量（否则如果直接使用列表中的变量，会出现Index out of range的BUG）
            Vector2 spawnPos = enemySpawnList[i];
            GameObject enemyPrefab = doorController.EnemyObjects[i];

            //打印出索引和临时变量的坐标
            Debug.Log($"Spawning warning for enemy at index {i} with position {spawnPos}");


            GenerateSpawnWarningObject(() =>
            {
                //打印出索引和变量在传递给匿名函数后的数值（索引i与上方的不一致，导致BUG）
                Debug.Log($"Inside lambda: Spawning enemy at index {i} with position {spawnPos}");
                LogicPassToSpawnWarningObject(enemyPrefab, spawnPos, doorController);
            }, spawnPos);
        }
    }




    private void LogicPassToSpawnWarningObject(GameObject thisObject, Vector2 spawnPos, DoorController doorController)
    {
        //这里的enemy物体是敌人的跟物体（包含巡逻坐标的），在生成的同时赋予物体生成坐标
        GameObject enemyObject = EnemyPool.Instance.GetObject(thisObject, spawnPos);     //从敌人对象池中生成敌人

        InitializeEnemy(enemyObject, doorController);
    }
   

    //生成完敌人后，进行初始化
    public void InitializeEnemy(GameObject enemyObject, DoorController doorController)
    {
        Enemy enemyScript = enemyObject.GetComponentInChildren<Enemy>();
        if (enemyScript == null)
        {
            Debug.LogError("Cannot get the Enemy reference in the " + enemyObject.name);
            return;
        }

        //重置敌人脚本绑定的物体的本地（相对于父物体）坐标。因为敌人从对象池重新生成后，本地坐标会继承死亡前的本地坐标
        enemyScript.ResetLocalPos();

        //设置门控制器的脚本
        enemyScript.SetDoorController(doorController);

        //生成敌人后重置生命，否则重新激活的敌人生命依然为0
        Stats enemyStats = enemyObject.GetComponentInChildren<Stats>();
        if (enemyStats == null)
        {
            Debug.LogError("Cannot get the Stats reference in the " + enemyObject.name);
            return;
        }

        enemyStats.SetCurrentHealth(enemyStats.MaxHealth);
    }
    #endregion


    #region 检查函数
    //检查列表中的所有坐标处是否有家具
    private void EnsureNoFurnitureCollision(List<Vector2> enemySpawnPosList, DoorController doorController)
    {
        Vector2 checkSize = new Vector2(doorController.PhysicsCheckingXPos, doorController.PhysicsCheckingYPos);      //物理检测的大小

        float adaptiveTolerance = doorController.EnemySpwanPos.GetOverlapTolerance();        //获取检查重复的距离
        int attemptCount = 0;           //用于防止进入无限循环的变量


        while (attemptCount < 100)      //确保不超过最大尝试次数
        {
            bool isOverlap = false;

            for (int i = 0; i < enemySpawnPosList.Count; i++)
            {
                if (!IsPositionEmpty(enemySpawnPosList[i], checkSize, doorController))
                {
                    enemySpawnPosList[i] = doorController.EnemySpwanPos.GenerateNonOverlappingPosition(enemySpawnPosList);

                    doorController.EnemySpwanPos.SetOverlapTolerance(adaptiveTolerance);     //设置新的检查重复的距离

                    isOverlap = true;   //设置布尔以继续检查
                }
            }

            if (!isOverlap) break;      //当没有重复时则退出循环

            attemptCount++;
            adaptiveTolerance -= 0.1f;  //如果实在难以生成不会重复的坐标的话，减少检查重复的距离
        }

        //当超出最大尝试次数后依然没有生成合适的坐标时，则进行提醒
        if (attemptCount >= 100)
        {
            Debug.Log("Attempt counts already exceeds the max allowed counts!");
        }
    }

    //检查单独的坐标是否跟家具重叠
    private void EnsureNoFurnitureCollision(ref Vector2 enemySpawnPos, DoorController doorController)
    {
        Vector2 checkSize = new Vector2(doorController.PhysicsCheckingXPos, doorController.PhysicsCheckingYPos);      //物理检测的大小

        float adaptiveTolerance = doorController.EnemySpwanPos.GetOverlapTolerance();        //获取检查重复的距离
        int attemptCount = 0;       //用于防止进入无限循环的变量


        while (attemptCount < 100)      //确保不超过最大尝试次数
        {
            //检查是否跟家具重复，不重复的话就退出循环
            if (IsPositionEmpty(enemySpawnPos, checkSize, doorController)) break;


            enemySpawnPos = doorController.EnemySpwanPos.GenerateSingleRandomPos();      //生成新的坐标

            doorController.EnemySpwanPos.SetOverlapTolerance(adaptiveTolerance);     //设置新的检查重复的距离                  

            attemptCount++;
            adaptiveTolerance -= 0.1f;  //如果实在难以生成不会重复的坐标的话，减少检查重复的距离
        }

        //当超出最大尝试次数后依然没有生成合适的坐标时，则进行提醒
        if (attemptCount >= 100)
        {
            Debug.Log("Attempt counts already exceeds the max allowed counts!");
        }
    }


    //运用物理函数检查要生成的坐标是否有家具
    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 checkSize, DoorController doorController)
    {
        //第一个参数为中心点，第二个参数为长方形大小（沿着中心各延申一半），第三个参数为角度，第四个参数为检测的目标层级
        Collider2D overlapCheck = Physics2D.OverlapBox(positionToCheck, checkSize, 0f, doorController.FurnitureLayerMask);
        return overlapCheck == null;
    }

    //该函数跟上面一样，只是第三个参数不同
    public bool IsPositionEmpty(Vector2 positionToCheck, Vector2 checkSize, LayerMask checkedLayer)
    {
        Collider2D overlapCheck = Physics2D.OverlapBox(positionToCheck, checkSize, 0f, checkedLayer);
        return overlapCheck == null;
    }
    #endregion


    #region 其余函数
    //每当加载新场景时调用的函数
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //进入主菜单
        if (scene.name == SceneManagerScript.MainMenuSceneName)
        {
            //重置布尔
            IsGameLost = false;
        }

        //进入一楼场景
        else if (scene.name == SceneManagerScript.FirstFloorSceneName)
        {
            //将布尔设置为false，防止玩家重新游戏后重复打开游戏背景介绍
            IsFirstTimeEnterGame = false;
        }

        else
        {
            Debug.Log("We only have two scenes now, please check the parameters!");
        }
    }

    //重置游戏
    public void ResetGame()
    {
        //重置布尔
        IsGameLost = false;
    }
    #endregion


    #region Setters
    public void SetIsGameLost(bool isTrue)
    {
        IsGameLost = isTrue;
    }
    #endregion
}
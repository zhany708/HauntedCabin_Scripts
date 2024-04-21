using UnityEngine;


public class EnvironmentManager : MonoBehaviour     //用于处理游戏过程中的一些动态变化（比如在某个地方生成某个新东西）
{
    public static EnvironmentManager Instance { get; private set; }


    public GameObject BlockDoorBarrel;      //挡住玩家进入门的障碍物







    private void Awake()
    {
        //单例模式
        if (Instance == null)
        {
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
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
}
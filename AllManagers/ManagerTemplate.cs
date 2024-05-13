using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;



public abstract class ManagerTemplate<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }


    //预制件缓存字典，所有的管理器加载完物体后都会保存进这个字典（每个管理器都有一个单独且分开的字典，只是名字一样）
    protected Dictionary<string, GameObject> m_PrefabDict = new Dictionary<string, GameObject>();




    protected virtual void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this as T)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this as T;

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }         
        }
    }





    //异步加载
    protected async Task<GameObject> LoadPrefabAsync(string name)
    {
        //检查字典里是否有预制件，如果有的话直接返回
        if (!m_PrefabDict.TryGetValue(name, out GameObject panelPrefab))
        {
            //异步加载游戏物体
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            await handle.Task;

            //检查异步加载是否成功
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                panelPrefab = handle.Result;

                //将预制件存进字典
                m_PrefabDict[name] = panelPrefab;
            }

            else
            {
                Debug.LogError($"Failed to load prefab: {name}");
                return null;
            }
        }

        return panelPrefab;
    }



    //在Addressables里释放物体，只有这样才能释放内存
    public void ReleasePrefab(string key)
    {
        if (key.EndsWith("(Clone)"))
        {
            //检查是否有“克隆”后缀，如果有的话减去后缀。（Clone）刚好有7个字符
            key = key.Substring(0, key.Length - 7);
        }


        if (m_PrefabDict.TryGetValue(key, out GameObject gameObjectPrefab))
        {
            Addressables.Release(gameObjectPrefab);

            //从预制件缓存字典中移除物体
            m_PrefabDict.Remove(key);

            //Debug.Log("Gameobject released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This GameObject is not loaded yet, cannot release: " + key);
        }
    }





    //设置脚本中跟物体的坐标
    protected void SetupRootGameObject(ref Transform rootGameObject, string rootGameObjectName)
    {
        //尝试寻找游戏中含有第二个参数的名字的物体，如果没找到的话就新建一个
        GameObject rootObject = GameObject.Find(rootGameObjectName) ?? new GameObject(rootGameObjectName);

        rootGameObject = rootObject.transform;
    }
}
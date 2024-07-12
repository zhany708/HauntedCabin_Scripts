using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



public class ParticlePool : MonoBehaviour       //用于子弹，特效等的对象池。生成出来的所有物体都放在子物体
{
    public static ParticlePool Instance {  get; private set; }


    //使用字典对不同的物体进行分开存储
    private Dictionary<string, Queue<GameObject>> m_ParticlePoolDict = new Dictionary<string, Queue<GameObject>>();     








    #region Unity内部函数
    private void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;
        }
    }
    #endregion


    #region 主要函数
    //获取物体
    public GameObject GetObject(GameObject prefab)
    {
        //检查池中有没有物体，没有的话则新建一个并加进去
        if (!m_ParticlePoolDict.TryGetValue(prefab.name, out var queue) || queue.Count == 0)
        {
            var newObject = CreateNewObject(prefab);
            PushObject(newObject);
        }

        var obj = m_ParticlePoolDict[prefab.name].Dequeue();
        obj.SetActive(true);

        return obj;
    }



    private GameObject CreateNewObject(GameObject prefab)
    {
        //先找用来存放每类物体的父物体，随后再创建
        GameObject childContainer = FindOrCreateChildContainer(prefab.name);
        GameObject obj = Instantiate(prefab, childContainer.transform);

        return obj;
    }

    //用于寻找或创建子物体的父物体（为了整洁美观）
    private GameObject FindOrCreateChildContainer(string name)
    {
        //先尝试在当前物体的子物体中寻找参数中的名字的物体
        Transform childTransform = transform.Find(name);

        if (childTransform == null)
        {
            GameObject child = new GameObject(name);

            //将每种类型的物体的父物体设置为当前物体的子物体
            child.transform.SetParent(transform);

            return child;
        }

        return childTransform.gameObject;
    }





    //储存物体
    public bool PushObject(GameObject obj)
    {
        if (obj != null)
        {
            //获取物体名字
            string name = obj.name;

            if (name.EndsWith("(Clone)"))
            {
                //检查是否有“克隆”后缀，如果有的话减去后缀。（Clone）刚好有7个字符
                name = name.Substring(0, name.Length - 7);
            }



            if (!m_ParticlePoolDict.ContainsKey(name))
            {
                //如果池中没有物体，则创建一个并加进去
                m_ParticlePoolDict[name] = new Queue<GameObject>();
            }

            m_ParticlePoolDict[name].Enqueue(obj);

            //创建完后取消激活
            obj.SetActive(false);

            //随后设置父物体
            obj.transform.SetParent(FindOrCreateChildContainer(name).transform);

            return true;
        }
     
        return false;
    }
    #endregion


    #region 其余函数（重置游戏等）
    //每当加载新场景时调用的函数
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //进入主菜单
        if (scene.name == SceneManagerScript.MainMenuSceneName)
        {
            //重置游戏
            ResetGame();
        }
    }

    public void ResetGame()     //重置游戏（在返回主菜单前调用此函数）
    {
        //在场景中取消激活所有对象池中的物体
        foreach (Transform child in transform)
        {
            foreach (Transform child2 in child)     //检索每一个子物体的子物体
            {
                //如果任何物体处于激活状态，则放回池中
                if (child2.gameObject.activeSelf)
                {
                    //这里必须放回池中，不能单纯的取消激活。否则在重置游戏后，将不会重复使用之前生成过的物体
                    PushObject(child2.gameObject);      //将物体放回池中
                }
            }
        }
    }
    #endregion
}
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour       //�������ɵ��˵Ķ����
{
    public static EnemyPool Instance { get; private set; }


    Dictionary<string, Queue<GameObject>> m_EnemyPool = new Dictionary<string, Queue<GameObject>>();

    GameObject m_Pool;






    private void Awake()
    {
        //����ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;

            m_Pool = new GameObject("EnemyPool");
        }

    }



    //��ȡ����
    public GameObject GetObject(GameObject prefab)
    {
        //��������û�����壬û�еĻ����½�һ�����ӽ�ȥ
        if (!m_EnemyPool.TryGetValue(prefab.name, out var queue) || queue.Count == 0)
        {
            var newObject = CreateNewObject(prefab);
            PushObject(newObject);
        }

        var obj = m_EnemyPool[prefab.name].Dequeue();
        obj.SetActive(true);

        return obj;

    }



    private GameObject CreateNewObject(GameObject prefab)
    {
        //���Ҹ����壬����ٴ���
        GameObject childContainer = FindOrCreateChildContainer(prefab.name);
        GameObject obj = Instantiate(prefab, childContainer.transform);

        return obj;
    }

    //����Ѱ�һ򴴽�������ĸ����壨Ϊ���������ۣ�
    private GameObject FindOrCreateChildContainer(string name)
    {
        Transform childTransform = m_Pool.transform.Find(name);

        if (childTransform == null)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(m_Pool.transform);

            return child;
        }

        return childTransform.gameObject;
    }





    //��������
    public bool PushObject(GameObject obj)
    {
        if (obj != null)
        {
            //ȥ���������ֵĺ�׺
            string name = obj.name.Replace("(Clone)", "").Trim();

            if (!m_EnemyPool.ContainsKey(name))
            {
                //�������û�����壬�򴴽�һ�����ӽ�ȥ
                m_EnemyPool[name] = new Queue<GameObject>();
            }

            m_EnemyPool[name].Enqueue(obj);

            //�������ȡ������
            obj.SetActive(false);
            obj.transform.SetParent(FindOrCreateChildContainer(name).transform);

            return true;
        }

        return false;
    }
}

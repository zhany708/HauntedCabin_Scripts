using System.Collections.Generic;
using UnityEngine;

public class EnemyPool       //�������ɵ��˵Ķ����
{
    public static EnemyPool Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new EnemyPool();
            }
            return m_Instance;
        }
    }
    private static EnemyPool m_Instance;

    Dictionary<string, Queue<GameObject>> m_EnemyPool = new Dictionary<string, Queue<GameObject>>();

    GameObject m_Pool;



    public GameObject GetObject(GameObject prefab, Vector2 spawnPos)
    {
        GameObject enemyObject;

        if (!m_EnemyPool.ContainsKey(prefab.name) || m_EnemyPool[prefab.name].Count == 0)       //�������Ƿ������壬�����������Ƿ�Ϊ0
        {
            enemyObject = GameObject.Instantiate(prefab);
            PushObject(enemyObject);

            if (m_Pool == null)
            {
                m_Pool = new GameObject("EnemyPool");   //���ɴ������гض�����ܸ�����
            }

            GameObject m_Child = GameObject.Find(prefab.name);
            if (!m_Child)
            {
                m_Child = new GameObject(prefab.name);     //����ÿ���ض���ĸ�����
                m_Child.transform.SetParent(m_Pool.transform);      //��ÿ���ض���ĸ���������Ϊ�ܸ������������
            }

            enemyObject.transform.SetParent(m_Child.transform);     //��ÿ����������Ϊ�ض���ĸ������������
        }

        enemyObject = m_EnemyPool[prefab.name].Dequeue();       //����Ԥ��������ȡ������е�Ԥ����
        enemyObject.GetComponentInChildren<Enemy>().SetSpawnPos(spawnPos);      //���������괫�����ˣ���ȷ��Ѳ������
        enemyObject.SetActive(true); 
        return enemyObject;       
    }

    public void PushObject(GameObject prefab)
    {
        string enemyName = prefab.name.Replace("(Clone)", string.Empty);    //����¡��׺�滻�ɿ�

        if (!m_EnemyPool.ContainsKey(enemyName))
        {
            m_EnemyPool.Add(enemyName, new Queue<GameObject>());    //�����������Ƿ�����ڶ���أ�����������������һ��
        }
        m_EnemyPool[enemyName].Enqueue(prefab);     //���ɺ������������

        prefab.SetActive(false);    //�����ȡ������
    }
}

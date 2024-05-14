using System.Collections.Generic;
using UnityEngine;



public class EnemyPool : MonoBehaviour       //�������ɵ��˵Ķ���أ����ɳ��������е��˶�����������
{
    public static EnemyPool Instance { get; private set; }


    Dictionary<string, Queue<GameObject>> m_EnemyPool = new Dictionary<string, Queue<GameObject>>();








    private void Awake()
    {
        //����ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }



    //��ȡ���壬�ڶ�������Ϊ���˵���������
    public GameObject GetObject(GameObject prefab, Vector2 spawnPos)
    {
        //��������û�����壬û�еĻ����½�һ�����ӽ�ȥ
        if (!m_EnemyPool.TryGetValue(prefab.name, out var queue) || queue.Count == 0)
        {
            var newObject = CreateNewObject(prefab, spawnPos);
            PushObject(newObject);
        }

        var obj = m_EnemyPool[prefab.name].Dequeue();

        //�����弤��ǰ������������˵ĸ����壬����������ȷ�ĳ�ʼ������Ѳ�ߵ�Ľű�
        obj.transform.position = spawnPos;

        obj.SetActive(true);

        return obj;
    }



    private GameObject CreateNewObject(GameObject prefab, Vector2 spawnPos)
    {
        //�����������ÿ����˵ĸ����壬����ٴ���
        GameObject childContainer = FindOrCreateChildContainer(prefab.name);
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, childContainer.transform);

        return obj;
    }

    //����Ѱ�һ򴴽�������ĸ����壨Ϊ���������ۣ�
    private GameObject FindOrCreateChildContainer(string name)
    {
        //�ȳ����ڵ�ǰ�������������Ѱ�Ҳ����е����ֵ�����
        Transform childTransform = transform.Find(name);

        if (childTransform == null)
        {
            GameObject child = new GameObject(name);

            //��ÿ�����͵ĵ��˵ĸ���������Ϊ��ǰ�����������
            child.transform.SetParent(transform);

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

            //������ø�����
            obj.transform.SetParent(FindOrCreateChildContainer(name).transform);

            return true;
        }

        return false;
    }



    public void ResetGame()     //������Ϸ���ڼ�����������ǰ���ô˺�����
    {
        //�ڳ�����ȡ���������е���
        foreach (Transform child in transform)    
        {
            foreach (Transform child2 in child)     //����ÿһ���������������
            {
                if (child2.CompareTag("Enemy"))
                {
                    //�������Żس��У����ܵ�����ȡ����������ڵ��˴��ʱ������Ϸ�󣬽������ظ�ʹ��֮ǰ���ɹ��ĵ�������
                    PushObject(child2.gameObject);      //�����˽ű��ĸ�����Żس��У�Ҳ���Żظ����������������
                }
            }
        }
    }

    public void EndGame()     //������Ϸ�������ʤ��ʱ���ô˺�����
    {
        //�ڳ����������м���ĵ��˽�������״̬
        foreach (Transform child in transform)
        {
            foreach (Transform child2 in child)     //����ÿһ���������������
            {
                //��������Ƿ��е��˱�ǩ���Ҵ��ڼ���״̬
                if (child2.CompareTag("Enemy") && child2.gameObject.activeSelf)
                {
                    Enemy enemyScript = child2.GetComponentInChildren<Enemy>();     //���������ǻ�ȡ���˽ű�

                    if (enemyScript == null)
                    {
                        Debug.LogError("Cannot get the Enemy script from the children Objects.");
                        return;
                    }

                    enemyScript.StateMachine.ChangeState(enemyScript.DeathState);   //ǿ���õ��˽�������״̬                 
                }
            }
        }
    }
}
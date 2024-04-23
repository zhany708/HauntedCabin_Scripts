using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;



public abstract class ManagerTemplate<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }


    //Ԥ�Ƽ������ֵ䣬���еĹ���������������󶼻ᱣ�������ֵ䣨ÿ������������һ�������ҷֿ����ֵ䣬ֻ������һ����
    protected Dictionary<string, GameObject> m_PrefabDict = new Dictionary<string, GameObject>();




    protected virtual void Awake()
    {
        //����ģʽ
        if (Instance != null && Instance != this as T)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this as T;
        }
    }





    //�첽����
    protected async Task<GameObject> LoadPrefabAsync(string name)
    {
        //����ֵ����Ƿ���Ԥ�Ƽ�������еĻ�ֱ�ӷ���
        if (!m_PrefabDict.TryGetValue(name, out GameObject panelPrefab))
        {
            //�첽������Ϸ����
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            await handle.Task;

            //����첽�����Ƿ�ɹ�
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                panelPrefab = handle.Result;

                //��Ԥ�Ƽ�����ֵ�
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



    //��Addressables���ͷ����壬ֻ�����������ͷ��ڴ�
    public void ReleasePrefab(string key)
    {
        if (key.EndsWith("(Clone)"))
        {
            //����Ƿ��С���¡����׺������еĻ���ȥ��׺����Clone���պ���7���ַ�
            key = key.Substring(0, key.Length - 7);
        }


        if (m_PrefabDict.TryGetValue(key, out GameObject gameObjectPrefab))
        {
            Addressables.Release(gameObjectPrefab);

            //��Ԥ�Ƽ������ֵ����Ƴ�����
            m_PrefabDict.Remove(key);

            Debug.Log("Gameobject released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This GameObject is not loaded yet, cannot release: " + key);
        }
    }
}
using UnityEngine;
using System.Threading.Tasks;




public class ScreenplayManager : ManagerTemplate<ScreenplayManager>     //���ڹ���籾��ص��߼���
{
    public SO_ScreenplayKeys ScreenplayKeys;






    public async Task OpenScreenplay(string name)       //�򿪾籾
    {
        //�첽���أ�������Ƿ���سɹ�
        GameObject screenPlayPrefab = await LoadPrefabAsync(name);
        if (screenPlayPrefab == null)
        {
            Debug.LogError("Failed to load screenplay prefab: " + name);
            return;
        }


        //�첽���غ��������岢��ȡ�������ϵ����������λ�ڳ�ʼ���꣬������ת��
        GameObject screenplayObject = GameObject.Instantiate(screenPlayPrefab, Vector3.zero, Quaternion.identity);

        if (screenplayObject == null)
        {
            Debug.LogError("ScreenplayObject instantiated fail: " + name);
            return;
        }
    }
}
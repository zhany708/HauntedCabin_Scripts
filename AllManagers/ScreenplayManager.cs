using UnityEngine;
using System.Threading.Tasks;




public class ScreenplayManager : ManagerTemplate<ScreenplayManager>     //用于管理剧本相关的逻辑的
{
    public SO_ScreenplayKeys ScreenplayKeys;






    public async Task OpenScreenplay(string name)       //打开剧本
    {
        //异步加载，随后检查是否加载成功
        GameObject screenPlayPrefab = await LoadPrefabAsync(name);
        if (screenPlayPrefab == null)
        {
            Debug.LogError("Failed to load screenplay prefab: " + name);
            return;
        }


        //异步加载后生成物体并获取物体身上的组件（物体位于初始坐标，且无旋转）
        GameObject screenPlayObject = GameObject.Instantiate(screenPlayPrefab, Vector3.zero, Quaternion.identity);
        BaseScreenplay screenPlay = screenPlayObject.GetComponent<BaseScreenplay>();
        if (screenPlay != null)
        {
            //获取组件后，开始剧本
            screenPlay.StartScreenplay();
        }

        else
        {
            Debug.LogError("No BaseScreenplay component found on gameobject: " + name);
            return;
        }
    }
}
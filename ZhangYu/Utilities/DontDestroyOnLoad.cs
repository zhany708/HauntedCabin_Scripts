using UnityEngine;



public class DontDestroyOnLoad : MonoBehaviour      //给所有在加载场景时禁止删除的物体使用
{
    private void Awake()
    {
        if (transform.parent == null)
        {
            DontDestroyOnLoad(this);
        }        
    }
}
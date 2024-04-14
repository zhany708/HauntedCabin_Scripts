using UnityEngine;

public class AllManagers : MonoBehaviour
{
    private void Awake()
    {
        //确保自己以及所有子物体不会在加载其他场景时因为意外被删除
        DontDestroyOnLoad(gameObject);      
    }
}

using UnityEngine;

public class AllManagers : MonoBehaviour
{
    public static AllManagers Instance;




    private void Awake()
    {
        //单例模式
        if (Instance == null)
        {
            Instance = this;

            //确保自己以及所有子物体不会在加载其他场景时因为意外被删除
            DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }          
    }
}
using UnityEngine;



public class BaseScreenplay<T> : MonoBehaviour where T : Component         //所有剧本的底层逻辑（共同点）
{
    public static T Instance { get; private set; }





    protected virtual void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this as T)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this as T;

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    private void Start()
    {
        StartScreenplay();      //开始剧本Setup
    }


    public virtual void StartScreenplay()       //剧本开始（剧本的Setup，比如生成一些东西等）
    {

    }



    private void Victory()     //胜利相关的逻辑
    {

    }

    private void Lose()        //失败相关的逻辑
    {

    }
}
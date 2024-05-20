using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BaseScreenplay : MonoBehaviour         //所有剧本的底层逻辑（共同点）
{





    private void Awake()
    {
        DontDestroyOnLoad(gameObject);      //加载场景时禁止删除剧本物体
    }




    public virtual void StartScreenplay()       //剧本开始
    {

    }



    private void Victory()     //胜利相关的逻辑
    {

    }

    private void Lose()        //失败相关的逻辑
    {

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Introduction：所有剧本的模板
 * Creator：Zhang Yu
*/

//Non-Generic Base Class
public class BaseScreenplay : MonoBehaviour
{
    public virtual Task StartScreenplay() { }       //剧本开始（剧本的Setup，比如生成一些东西等）

    public virtual void ResetGame() { }             //重置游戏


    public virtual void Victory() { }               //胜利相关的逻辑

    public virtual Task Lose() { }                  //失败相关的逻辑
}

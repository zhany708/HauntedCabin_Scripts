using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Introduction：
 * Creator：Zhang Yu
*/

//Non-Generic Base Class
public class BaseScreenplay : MonoBehaviour
{
    public virtual void StartScreenplay() { }       //剧本开始（剧本的Setup，比如生成一些东西等）

    public virtual void ResetGame() { }         //重置游戏


    public virtual void Victory() { }    //胜利相关的逻辑

    public virtual void Lose() { }        //失败相关的逻辑
}

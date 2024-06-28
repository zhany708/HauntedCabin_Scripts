using System.Threading.Tasks;
using UnityEngine;

/*
 * Introduction：所有剧本的模板
 * Creator：Zhang Yu
*/


//Non-Generic Base Class
public class BaseScreenplay : MonoBehaviour
{
    public virtual Task StartScreenplay() { return null; }      //剧本开始（剧本的Setup，比如生成一些东西等）

    public virtual void ResetGame() { }                         //重置游戏


    public virtual Task Victory() { return null; }              //胜利相关的逻辑

    public virtual Task Lose() { return null; }                 //失败相关的逻辑
}

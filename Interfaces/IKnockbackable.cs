using UnityEngine;



public interface IKnockbackable     //用于所有可以被击退的物体
{
    void KnockBack(float strength, Vector2 direction);      //被击退
}
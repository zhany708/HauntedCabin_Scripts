using UnityEngine;



public interface Idamageable        //用于所有可以被伤害的物体
{
    public void Damage(float amount, bool doesIgnoreDefense);      //减少生命值（第一个参数表示伤害量，第二个参数表示是否无视防御）

    //public void GetHit(Vector2 direction);     //受击瞬间执行的逻辑（比如转向，改变形态等）
}
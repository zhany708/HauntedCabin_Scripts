using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MeleeWeapon : Weapon
{
    public int CurrentAttackCounter
    {
        get => m_CurrentAttackCounter;
        protected set => m_CurrentAttackCounter = value >= WeaponData.AmountOfAttack ? 0 : value;
    }
    private int m_CurrentAttackCounter;        //表示武器的连击次数



    protected SO_MeleeWeaponData aggressiveWeaponData;
    protected CameraShake cameraShake;

    protected List<Idamageable> detectedDamageables = new List<Idamageable>();     //用于储存所有在攻击范围的碰撞体
    protected List<IKnockbackable> detectedKnockbackables = new List<IKnockbackable>();  //用于储存所有攻击范围内可击退的碰撞体







    protected override void Awake()
    {
        base.Awake();

        InitializeComponents();     //初始化组件
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        AddToDetected(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        RemoveFromDetected(collision);
    }






    private void InitializeComponents()
    {
        //如果WeaponData与当前AggressiveWeaponData相同，则将当前攻击性武器数据的Reference传给Weapon脚本
        if (WeaponData.GetType() == typeof(SO_MeleeWeaponData))
        {
            aggressiveWeaponData = (SO_MeleeWeaponData)WeaponData;
        }
        else
        {
            Debug.LogError("Wrong data for the weapon");
            enabled = false;    //取消激活当前脚本（不是物体）
        }


        cameraShake = FindObjectOfType<CameraShake>();    //找拥有CameraShake脚本的组件
        if (cameraShake == null)
        {
            Debug.LogError("Cannot find CameraShake component.");
            return;
        }
    }




    public void CheckMeleeAttack()     //攻击到敌人时调用此函数
    {
        //Debug.Log("Checking!");

        //调用攻击性武器中不同连击次数的信息
        MeleeWeaponAttackDetails details = aggressiveWeaponData.AttackDetails[CurrentAttackCounter];

        //对每一个有可造成伤害接口的碰撞体生效，加ToList防止敌人死亡后出现Bug（ToList可以复制原始List）
        foreach (Idamageable item in detectedDamageables.ToList())      
        {
            if (cameraShake != null)
            {
                cameraShake.ShakeCamera(details.CameraShakeIntensity, details.CameraShakeDuration);     //调用相机震动脚本
            }

            item.Damage(details.DamageAmount);      //对被检测到碰撞体造成伤害
            //item.GetHit(Movement.FacingDirection);      //使被检测碰撞体面向玩家
        }

        foreach (IKnockbackable item in detectedKnockbackables.ToList())
        {
            item.KnockBack(details.KnockbackStrength, mousePosition.normalized);       //击退目标,击退方向为鼠标方向（武器指向的方向）
            
        }
    }


    public void AddToDetected(Collider2D collision)
    {
        //Debug.Log("Added!");
        if (collision.TryGetComponent<Idamageable>(out var damageable))
        {
            detectedDamageables.Add(damageable);     //如果检测到可造成伤害的碰撞体，则加进List
        }

        
        if (collision.TryGetComponent<IKnockbackable>(out var knockbackable))
        {
            detectedKnockbackables.Add(knockbackable);    //如果检测到可被击退的碰撞体，则加进List
        }
    }

    public void RemoveFromDetected(Collider2D collision)
    {
        //Debug.Log("Removed!");
        if (collision.TryGetComponent<Idamageable>(out var damageable))
        {
            detectedDamageables.Remove(damageable);
        }


        if (collision.TryGetComponent<IKnockbackable>(out var knockbackable))
        {
            detectedKnockbackables.Remove(knockbackable);
        }
    }

    #region Animation Events
    protected override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        CheckMeleeAttack();
    }
    #endregion
}
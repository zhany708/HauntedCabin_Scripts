using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZhangYu.Utilities;

public class GunWeapon : Weapon
{
    public GameObject BulletPrefab;
    public SO_GunData GunData {  get; private set; }



    protected Transform muzzlePos;



    Flip m_GunFlip;





    protected override void Awake()
    {
        base.Awake();

        if (WeaponData.GetType() == typeof(SO_GunData))
        {
            GunData = (SO_GunData)WeaponData;     //如果当前WeaponData与GunWeaponData相同，则用父类中的通用WeaponData赋值此脚本中的枪械武器数据
        }
        else
        {
            Debug.LogError("Wrong data for the weapon");
        }
    }

    protected override void Start()
    {
        base.Start();

        m_GunFlip = new Flip(transform);

        muzzlePos = transform.Find("Muzzle");
    }



    public override void EnterWeapon()
    {
        base.EnterWeapon();

        Fire();
    }



    protected override void PointToMouse()
    {
        base.PointToMouse();

        int flipNum = player.InputHandler.ProjectedMousePos.x < transform.position.x ? -1 : 1;    //当鼠标位于枪械左侧时，翻转枪械的Y值

        m_GunFlip.FlipY(flipNum);
    }




    private void Fire()
    {
        GameObject bullet = ParticlePool.Instance.GetObject(BulletPrefab);

        float offsetAngle = Random.Range(-5f, 5f);      //用于小幅偏移子弹（不让子弹完全对着鼠标发射）

        bullet.transform.position = muzzlePos.position;     //生成子弹后更改位置于枪口位置
        bullet.GetComponent<PlayerBullet>().SetWeapon(this);
        bullet.GetComponent<PlayerBullet>().SetSpeed(Quaternion.AngleAxis(offsetAngle, Vector3.forward) * mousePosition);     //使子弹向鼠标位置移动，并产生随机的角度偏移

        PlayAudioSound();   //播放开枪音效
    }
}

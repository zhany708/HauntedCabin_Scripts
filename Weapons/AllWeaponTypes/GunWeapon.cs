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

        InitializeComponents();     //初始化组件
    }





    private void InitializeComponents()
    {
        m_GunFlip = new Flip(transform);

        //如果当前WeaponData与GunWeaponData相同，则用父类中的通用WeaponData赋值此脚本中的枪械武器数据
        if (WeaponData.GetType() == typeof(SO_GunData))
        {
            GunData = (SO_GunData)WeaponData;
        }
        else
        {
            Debug.LogError("WeaponData assigned to GunWeapon is not of type SO_GunData");
            return;
        }


        muzzlePos = transform.Find("Muzzle");
        if (muzzlePos == null)
        {
            Debug.LogError("Muzzle position not found in the Gun prefab.");
            return;
        }
    }




    public override void EnterWeapon()
    {
        base.EnterWeapon();

        Fire();
    }



    protected override void PointToMouse()
    {
        base.PointToMouse();

        //当鼠标位于枪械左侧时，翻转枪械的Y值
        int flipNum = player.InputHandler.ProjectedMousePos.x < transform.position.x ? -1 : 1;    

        m_GunFlip.FlipY(flipNum);
    }




    private void Fire()
    {
        if (muzzlePos == null) return;

        GameObject bulletObject = ParticlePool.Instance.GetObject(BulletPrefab);    //从对象池获取子弹物体
        if (bulletObject == null)
        {
            Debug.LogError("Failed to retrieve bullet from ParticlePool.");
            return;
        }


        float offsetAngle = Random.Range(-5f, 5f);      //用于小幅偏移子弹（不让子弹完全对着鼠标发射）

        bulletObject.transform.position = muzzlePos.position;     //生成子弹后更改位置于枪口位置

        //从子弹物体那获取子弹脚本
        PlayerBullet bulletScript = bulletObject.GetComponent<PlayerBullet>();
        if (bulletScript == null)
        {
            Debug.LogError("Failed to get the PlayerBullet script.");
            return;
        }

        bulletScript.SetWeapon(this);

        //使子弹向鼠标位置移动，并产生随机的角度偏移
        bulletScript.SetSpeed(Quaternion.AngleAxis(offsetAngle, Vector3.forward) * mousePosition);     

        PlayAudioSound();   //播放开枪音效
    }
}
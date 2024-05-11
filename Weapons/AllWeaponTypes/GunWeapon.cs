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

        InitializeComponents();     //��ʼ�����
    }





    private void InitializeComponents()
    {
        m_GunFlip = new Flip(transform);

        //�����ǰWeaponData��GunWeaponData��ͬ�����ø����е�ͨ��WeaponData��ֵ�˽ű��е�ǹе��������
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

        //�����λ��ǹе���ʱ����תǹе��Yֵ
        int flipNum = PlayerInputHandler.Instance.ProjectedMousePos.x < transform.position.x ? -1 : 1;    

        m_GunFlip.FlipY(flipNum);
    }




    private void Fire()
    {
        if (muzzlePos == null) return;

        GameObject bulletObject = ParticlePool.Instance.GetObject(BulletPrefab);    //�Ӷ���ػ�ȡ�ӵ�����
        if (bulletObject == null)
        {
            Debug.LogError("Failed to retrieve bullet from ParticlePool.");
            return;
        }


        float offsetAngle = Random.Range(-5f, 5f);      //����С��ƫ���ӵ��������ӵ���ȫ������귢�䣩

        bulletObject.transform.position = muzzlePos.position;     //�����ӵ������λ����ǹ��λ��

        //���ӵ������ǻ�ȡ�ӵ��ű�
        PlayerBullet bulletScript = bulletObject.GetComponent<PlayerBullet>();
        if (bulletScript == null)
        {
            Debug.LogError("Failed to get the PlayerBullet script.");
            return;
        }

        bulletScript.SetWeapon(this);

        //ʹ�ӵ������λ���ƶ�������������ĽǶ�ƫ��
        bulletScript.SetSpeed(Quaternion.AngleAxis(offsetAngle, Vector3.forward) * mousePosition);     

        PlayAudioSound();   //���ſ�ǹ��Ч
    }
}
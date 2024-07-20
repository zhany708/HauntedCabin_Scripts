using System;
using UnityEngine;
using ZhangYu.Utilities;




public class Weapon : MonoBehaviour
{
    #region 组件
    public event Action OnWeaponExit;      //接受事件方为PlayerAttackState脚本

    public SO_WeaponData WeaponData;


    protected Animator animator;
    //protected AudioSource audioSource;        //挂载在武器物体上的播放器

    protected Core core;
    protected Player player;
    protected Flip weaponInventoryFlip;

    //检查m_Movement是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    protected Movement Movement => m_Movement ? m_Movement : core.GetCoreComponent(ref m_Movement);   
    private Movement m_Movement;
    #endregion


    #region 变量
    public Vector2 mousePosition { get; private set; }     //鼠标的方向
    #endregion


    #region Unity内部函数
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        //audioSource = GetComponent<AudioSource>();

        player = GetComponentInParent<Player>();

        if (player != null )
        {
            core = player.GetComponentInChildren<Core>();   //先调用Player父物体，然后再从父物体中寻找Core子物体
        }       
    }

    protected async virtual void OnEnable()
    {
        //确保已经赋值过音频名，且音频还没加载过
        if (WeaponData.AudioClipName.Length > 0 && !SoundManager.Instance.AudioDict.ContainsKey(WeaponData.AudioClipName))
        {
            //提前加载武器的攻击音效
            await SoundManager.Instance.LoadClipAsync(WeaponData.AudioClipName);
        }
    }

    protected virtual void Start()
    {
        weaponInventoryFlip = new Flip(transform.parent.transform);     //用武器库的坐标构造Flip脚本
    }

    protected virtual void Update()
    {
        //允许玩家攻击时，让武器持续指向鼠标
        if (BasePanel.IsPlayerAttackable)
        {
            PointToMouse();
        }
    }

    private void OnDestroy() 
    {
        //确保已经赋值过音频名，且音频已加载过
        if (WeaponData.AudioClipName.Length > 0 && SoundManager.Instance.AudioDict.ContainsKey(WeaponData.AudioClipName) )
        {
            //武器摧毁时释放武器的攻击音效
            SoundManager.Instance.ReleaseAudioClip(WeaponData.AudioClipName);
        }           
    }
    #endregion


    #region 主要函数
    public virtual void EnterWeapon()
    {
        //Debug.Log("Enter the weapon!");

        animator.SetBool("Attack", true);
    }

    public virtual void ExitWeapon()
    {
        animator.SetBool("Attack", false);

        OnWeaponExit?.Invoke();
    }




    protected virtual void PointToMouse()
    {
        mousePosition = (PlayerInputHandler.Instance.ProjectedMousePos - new Vector2(transform.parent.position.x, transform.parent.position.y));    //计算需要朝向鼠标的方向

        transform.parent.right = mousePosition.normalized;   //归一化后，更改武器库的朝向，而不是武器的
        weaponInventoryFlip.FlipX(player.FacingNum);       //实时翻转武器，防止玩家翻转时武器也被翻转
    }



    protected virtual void PlayAudioSound()     //播放武器攻击音效
    {
        if (!string.IsNullOrEmpty(WeaponData.AudioClipName) )
        {
            SoundManager.Instance.PlaySFXAsync(WeaponData.AudioClipName, WeaponData.AudioVolume);          
        }      
    }
    #endregion


    #region 动画帧事件
    protected virtual void AnimationActionTrigger() { }
    
    private void AnimationFinishTrigger()
    {
        ExitWeapon();
    }
    #endregion
}
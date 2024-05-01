using System;
using UnityEngine;
using ZhangYu.Utilities;




public class Weapon : MonoBehaviour
{
    public event Action OnExit;      //接受事件方为PlayerAttackState脚本

    #region Components
    //public AudioClip WeaponAudio;     //除非武器有多种攻击音效，否则无需在脚本中声明，可以直接放进AudioSource

    public SO_WeaponData WeaponData;


    protected Animator animator;
    protected AudioSource audioSource;

    protected Core core;
    protected Player player;
    protected Flip weaponInventoryFlip;

    protected Movement Movement => m_Movement ? m_Movement : core.GetCoreComponent(ref m_Movement);   //检查m_Movement是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    private Movement m_Movement;
    #endregion

    #region Variables
    public Vector2 mousePosition { get; private set; }     //鼠标的方向
    #endregion

    #region Unity Callback Functions
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GetComponentInParent<Player>();
        core = player.GetComponentInChildren<Core>();   //先调用Player父物体，然后再从父物体中寻找Core子物体
    }

    protected virtual void Start()
    {
        weaponInventoryFlip = new Flip(transform.parent.transform);     //用武器库的坐标构造Flip脚本
    }

    protected virtual void Update()
    {
        //游戏正常时，让武器持续指向鼠标
        if (!PauseMenuPanel.IsGamePaused)
        {
            PointToMouse();
        }
    }
    #endregion

    #region Other Functions
    public virtual void EnterWeapon()
    {
        //Debug.Log("Enter the weapon!");

        animator.SetBool("Attack", true);
    }

    public virtual void ExitWeapon()
    {
        animator.SetBool("Attack", false);

        OnExit?.Invoke();
    }




    protected virtual void PointToMouse()
    {
        mousePosition = (player.InputHandler.ProjectedMousePos - new Vector2(transform.parent.position.x, transform.parent.position.y));    //计算需要朝向鼠标的方向

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

    #region Animation Events
    protected virtual void AnimationActionTrigger() { }
    private void AnimationFinishTrigger()
    {
        ExitWeapon();
    }
    #endregion

    #region Setters
    public void SetMousePosition(Vector2 thisPos)
    {
        mousePosition = thisPos;
    }
    #endregion
}
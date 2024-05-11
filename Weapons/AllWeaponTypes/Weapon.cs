using System;
using UnityEngine;
using ZhangYu.Utilities;




public class Weapon : MonoBehaviour
{
    public event Action OnWeaponExit;      //�����¼���ΪPlayerAttackState�ű�

    #region Components
    public SO_WeaponData WeaponData;


    protected Animator animator;
    //protected AudioSource audioSource;        //���������������ϵĲ�����

    protected Core core;
    protected Player player;
    protected Flip weaponInventoryFlip;

    //���m_Movement�Ƿ�Ϊ�գ����ǵĻ��򷵻������ǵĻ������GetCoreComponent�����Ի�ȡ���
    protected Movement Movement => m_Movement ? m_Movement : core.GetCoreComponent(ref m_Movement);   
    private Movement m_Movement;
    #endregion

    #region Variables
    public Vector2 mousePosition { get; private set; }     //���ķ���
    #endregion

    #region Unity Callback Functions
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        //audioSource = GetComponent<AudioSource>();

        player = GetComponentInParent<Player>();

        if (player != null )
        {
            core = player.GetComponentInChildren<Core>();   //�ȵ���Player�����壬Ȼ���ٴӸ�������Ѱ��Core������
        }       
    }

    protected virtual void Start()
    {
        weaponInventoryFlip = new Flip(transform.parent.transform);     //������������깹��Flip�ű�
    }

    protected virtual void Update()
    {
        //��Ϸû����ͣʱ������������ָ�����
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

        OnWeaponExit?.Invoke();
    }




    protected virtual void PointToMouse()
    {
        mousePosition = (PlayerInputHandler.Instance.ProjectedMousePos - new Vector2(transform.parent.position.x, transform.parent.position.y));    //������Ҫ�������ķ���

        transform.parent.right = mousePosition.normalized;   //��һ���󣬸���������ĳ��򣬶�����������
        weaponInventoryFlip.FlipX(player.FacingNum);       //ʵʱ��ת��������ֹ��ҷ�תʱ����Ҳ����ת
    }



    protected virtual void PlayAudioSound()     //��������������Ч
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
    /*
    public void SetMousePosition(Vector2 thisPos)
    {
        mousePosition = thisPos;
    }
    */
    #endregion
}
using System;
using Cinemachine;
using UnityEngine;
using ZhangYu.Utilities;


public class Player : MonoBehaviour
{
    #region FSM States
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerAttackState PrimaryAttackState { get; private set; }
    public PlayerAttackState SecondaryAttackState { get; private set; }
    #endregion

    #region Components
    public CinemachineVirtualCamera PlayerCamera;
    public Animator FootAnimator { get; private set; }
    



    public Core Core { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Weapon PrimaryWeapon {  get; private set; }
    public Weapon SecondaryWeapon {  get; private set; }


    [SerializeField]
    public SO_PlayerData PlayerData;

    Flip m_PlayerFlip;
    #endregion

    #region Other Variable
    //���������������ڸı�����߶�
    public float ZoomSpeed = 0.5f;
    public float MinOrthoSize = 5.4f;
    public float MaxOrthoSize = 10f;


    public bool IsFirstFrame { get; private set; } = true;
    public int FacingNum { get; private set; }
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        FootAnimator = transform.Find("PlayerFoot").GetComponent<Animator>();   //��ȡ���ϵĶ��������

        Core = GetComponentInChildren<Core>();      //���������ǵ���Core�ű�
        Core.SetParameters(PlayerData.MaxHealth, PlayerData.Defense, PlayerData.HitResistance);   //����Ҳ�������Core

        PrimaryWeapon = transform.Find("PrimaryWeapon").GetComponentInChildren<Weapon>();
        SecondaryWeapon = transform.Find("SecondaryWeapon").GetComponentInChildren<Weapon>();


        StateMachine = new PlayerStateMachine();

        //��ʼ����״̬
        IdleState = new PlayerIdleState(this, StateMachine, PlayerData, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, PlayerData, "Idle");
        HitState = new PlayerHitState(this, StateMachine, PlayerData, "Hit");
        PrimaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", PrimaryWeapon);
        SecondaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", SecondaryWeapon);
    }

    private void Start()
    {
        InputHandler = GetComponent<PlayerInputHandler>();

        m_PlayerFlip = new Flip(transform);

        FacingNum = 1;  //��Ϸ��ʼʱ��ʼ��FacingNum������������ͼ�޷�������ʾ

        StateMachine.Initialize(IdleState);     //��ʼ��״̬Ϊ����
    }

    private void Update()
    {
        //Core.LogicUpdate();     //��ȡ��ǰ�ٶ�

        if (IsFirstFrame)       //��ֹ��һ֡��ɫ�쳣��ת��ToDO:����ÿ����ͣ��Ϸʱ��Ҳ��Ҫ��ֹ�ָ����һ֡��ɫ�쳣��ת
        {
            IsFirstFrame = false;
            return;
        }

        PlayerFlip();   //��������Ƿ�ת���
        ZoomCamera();   //��������Ľ����ߴ�

        StateMachine.currentState.LogicUpdate();    //�������õ�ǰ״̬���߼�����
    }

    private void FixedUpdate()
    {
        StateMachine.currentState.PhysicsUpdate();  //�������õ�ǰ״̬�������߼�����
    }
    #endregion

    #region Other Functions
    public void ChangeWeapon(GameObject weapon, bool isPrimary)
    {
        if (isPrimary)
        {
            PrimaryWeapon.gameObject.SetActive(false);  //ȡ�����ǰ����

            PrimaryWeapon = WeaponInventory.Instance.LoadWeapon(weapon.name, isPrimary);
            PrimaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", PrimaryWeapon);       //�����¹���״̬
        }
        else
        {
            SecondaryWeapon.gameObject.SetActive(false);

            SecondaryWeapon = WeaponInventory.Instance.LoadWeapon(weapon.name, isPrimary);
            SecondaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", SecondaryWeapon);
        }
    }




    private void PlayerFlip()
    {
        FacingNum = InputHandler.ProjectedMousePos.x < transform.position.x ? -1 : 1;     //����������λ�������࣬��ת���

        m_PlayerFlip.FlipX(FacingNum);
    }

    private void ZoomCamera()       //ͨ����������������Զ���
    {
        if (PlayerCamera != null)
        {
            float mouseScroll = InputHandler.MouseScrollInput.y;    //ʹ����������Ϣ��yֵ

            float newOrthoSize = PlayerCamera.m_Lens.OrthographicSize - mouseScroll * ZoomSpeed;
            newOrthoSize = Mathf.Clamp(newOrthoSize, MinOrthoSize, MaxOrthoSize);       //������µ������Ƭ�����ߴ籣����������С֮��

            PlayerCamera.m_Lens.OrthographicSize = newOrthoSize;    //������ɺ����µ�ֵ
        }
    }
    #endregion

    #region Animation Event Functions
    private void DestroyPlayerAfterDeath()      //���ڶ����¼����ݻ�����
    {
        Destroy(gameObject);   
    }
    #endregion
}
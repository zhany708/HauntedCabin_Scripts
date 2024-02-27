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
    public PlayerInventory Inventory { get; private set; }
    public Weapon PrimaryWeapon {  get; private set; }
    public Weapon SecondaryWeapon {  get; private set; }


    [SerializeField]
    public SO_PlayerData PlayerData;

    Flip m_PlayerFlip;
    #endregion

    #region Other Variable
    public float ZoomSpeed = 0.5f;
    public float MinOrthoSize = 5.4f;
    public float MaxOrthoSize = 10f;

    public int FacingNum { get; private set; }


    int m_CurrentPrimaryWeaponNum = 0;      //使角色游戏开始默认装备匕首
    int m_CurrentSecondaryWeaponNum = 0;

    bool m_FirstFrame = true;
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        FootAnimator = transform.Find("PlayerFoot").GetComponent<Animator>();   //获取脚上的动画器组件

        Core = GetComponentInChildren<Core>();      //从子物体那调用Core脚本
        Core.SetParameters(PlayerData.MaxHealth, PlayerData.Defense, PlayerData.HitResistance);   //将玩家参数传给Core

        PrimaryWeapon = transform.Find("PrimaryWeapon").GetComponentInChildren<Weapon>();
        SecondaryWeapon = transform.Find("SecondaryWeapon").GetComponentInChildren<Weapon>();


        StateMachine = new PlayerStateMachine();

        //初始化各状态
        IdleState = new PlayerIdleState(this, StateMachine, PlayerData, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, PlayerData, "Idle");
        HitState = new PlayerHitState(this, StateMachine, PlayerData, "Hit");
        PrimaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", PrimaryWeapon);
        SecondaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", SecondaryWeapon);
    }

    private void Start()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
        Inventory = GetComponent<PlayerInventory>();

        m_PlayerFlip = new Flip(transform);

        FacingNum = 1;  //游戏开始时初始化FacingNum，否则武器贴图无法正常显示

        StateMachine.Initialize(IdleState);     //初始化状态为闲置
    }

    private void Update()
    {
        //Core.LogicUpdate();     //获取当前速度

        if (m_FirstFrame)       //防止第一帧角色异常翻转。ToDO:后续每当暂停游戏时，也需要防止恢复后第一帧角色异常翻转
        {
            m_FirstFrame = false;
            return;
        }

        PlayerFlip();   //持续检测是否翻转玩家
        ZoomCamera();   //调整相机的矫正尺寸

        StateMachine.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.currentState.PhysicsUpdate();
    }
    #endregion

    #region Other Functions
    public void ChangeWeapon(GameObject weapon, bool isPrimary)
    {
        int newWeaponNum = CheckWeaponIndex(weapon);        //获取新的武器计数

        //将需要更换的武器通过SetActive激活，并根据主/副生成新的攻击状态
        if (isPrimary)      //激活新武器于主手
        {
            Inventory.PrimaryWeapon[m_CurrentPrimaryWeaponNum].SetActive(false);
            Inventory.PrimaryWeapon[newWeaponNum].SetActive(true);      

            PrimaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", Inventory.PrimaryWeapon[newWeaponNum].GetComponent<Weapon>());       //激活新攻击状态

            m_CurrentPrimaryWeaponNum = newWeaponNum;   //重新设置当前武器计数
        }

        else     //激活新武器于副手
        {
            Inventory.SecondaryWeapon[m_CurrentSecondaryWeaponNum].SetActive(false);
            Inventory.SecondaryWeapon[newWeaponNum].SetActive(true);      

            SecondaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", Inventory.SecondaryWeapon[newWeaponNum].GetComponent<Weapon>());  

            m_CurrentSecondaryWeaponNum = newWeaponNum;
        }
    }


    private int CheckWeaponIndex(GameObject weapon)    //主武器和副武器的顺序一样，所以无需区分计数
    {
        int WeaponNum = 0;
        for (int i = 0; i < Inventory.PrimaryWeapon.Length; i++)
        {
            if (weapon.ToString() == Inventory.PrimaryWeapon[i].ToString())
            {
                WeaponNum = i;
            }
        }

        return WeaponNum;
    }


    private void PlayerFlip()
    {
        FacingNum = InputHandler.ProjectedMousePos.x < transform.position.x ? -1 : 1;     //如果鼠标坐标位于玩家左侧，则翻转玩家

        m_PlayerFlip.FlipX(FacingNum);
    }

    private void ZoomCamera()       //通过鼠标滚轮拉近或拉远相机
    {
        if (PlayerCamera != null)
        {
            float mouseScroll = InputHandler.MouseScrollInput.y;    //使用鼠标滚轮信息的y值

            float newOrthoSize = PlayerCamera.m_Lens.OrthographicSize - mouseScroll * ZoomSpeed;
            newOrthoSize = Mathf.Clamp(newOrthoSize, MinOrthoSize, MaxOrthoSize);       //计算后将新的相机镜片矫正尺寸保持在最大和最小之间

            PlayerCamera.m_Lens.OrthographicSize = newOrthoSize;    //计算完成后赋予新的值
        }
    }
    #endregion

    #region Animation Event Functions
    private void DestroyPlayerAfterDeath()      //用于动画事件，摧毁物体
    {
        Destroy(gameObject);   
    }
    #endregion
}
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
    public PlayerDeathState DeathState { get; private set; }
    public PlayerAttackState PrimaryAttackState { get; private set; }
    public PlayerAttackState SecondaryAttackState { get; private set; }
    #endregion

    #region Components
    public CinemachineVirtualCamera PlayerCamera;
    public Animator FootAnimator { get; private set; }
    



    public Core Core { get; private set; }
    public Weapon PrimaryWeapon {  get; private set; }
    public Weapon SecondaryWeapon {  get; private set; }


    [SerializeField]
    public SO_PlayerData PlayerData;

    Flip m_PlayerFlip;
    #endregion

    #region Other Variable
    //以下三个变量用于改变相机高度
    public float ZoomSpeed = 0.5f;
    public float MinOrthoSize = 5.4f;
    public float MaxOrthoSize = 10f;

    
    //public static bool IsAttackable {  get; private set; }      //表示玩家是否可攻击，用于受击间隔
    public bool IsFirstFrame { get; private set; } = true;
    public int FacingNum { get; private set; }

    //当前状态，用于Debug
    string m_CurrentState;
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
        DeathState = new PlayerDeathState(this, StateMachine, PlayerData, "Death");
        PrimaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", PrimaryWeapon);
        SecondaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", SecondaryWeapon);
    }

    private void Start()
    {
        m_PlayerFlip = new Flip(transform);

        FacingNum = 1;  //游戏开始时初始化FacingNum，否则武器贴图无法正常显示

        MakeSpriteVisible(SecondaryWeapon.transform.gameObject, false);       //在游戏开始前隐藏副武器

        StateMachine.Initialize(IdleState);     //初始化状态为闲置
    }

    private void Update()
    {
        //实时显示当前状态
        m_CurrentState = StateMachine.currentState.ToString();
        //Core.LogicUpdate();     //获取当前速度

        if (IsFirstFrame)       //防止第一帧角色异常翻转。ToDO:后续每当暂停游戏时，也需要防止恢复后第一帧角色异常翻转
        {
            IsFirstFrame = false;
            return;
        }

        PlayerFlip();   //持续检测是否翻转玩家
        ZoomCamera();   //调整相机的矫正尺寸

        StateMachine.currentState.LogicUpdate();    //持续调用当前状态的逻辑函数
    }

    private void FixedUpdate()
    {
        StateMachine.currentState.PhysicsUpdate();  //持续调用当前状态的物理逻辑函数
    }
    #endregion

    #region Other Functions
    //更换武器
    public async void ChangeWeapon(string weaponName, bool isPrimary)
    {
        bool isPrimaryAttackState = false;    //检查更换武器时是否处于要放置的攻击状态（主还是副）

        if (PrimaryWeapon.transform.gameObject.GetComponent<SpriteRenderer>().color == new Color(1f, 1f, 1f, 1f))       //通过透明度检查处于哪个攻击状态
        {
            isPrimaryAttackState = true;
        }



        if (isPrimary)
        {
            //更换武器之前退出当前武器攻击状态状态，防止出现Bug
            //PrimaryWeapon.ExitWeapon();

            if (PrimaryWeapon != null && PrimaryWeapon.gameObject.activeSelf)
            {
                PrimaryWeapon.gameObject.SetActive(false);      //换武器前先取消激活当前武器
            }

            await WeaponManager.Instance.LoadWeapon(weaponName, isPrimary);    //等待异步加载


            PrimaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", PrimaryWeapon);       //激活新攻击状态

            if (!isPrimaryAttackState)
            {
                MakeSpriteVisible(PrimaryWeapon.transform.gameObject, false);       //如果当前不在主武器攻击状态，则换新武器后再次隐藏
            }
        }

        else
        {
            if (SecondaryWeapon != null && SecondaryWeapon.gameObject.activeSelf)
            {
                SecondaryWeapon.gameObject.SetActive(false);
            }

            await WeaponManager.Instance.LoadWeapon(weaponName, isPrimary);


            SecondaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", SecondaryWeapon);

            if (isPrimaryAttackState)
            {
                MakeSpriteVisible(SecondaryWeapon.transform.gameObject, false);     //如果当前不在副武器攻击状态，则换新武器后再次隐藏
            }
        }

        //StateMachine.ChangeState(IdleState);
    }

    

    //更改渲染的透明度以激活/隐藏物体
    public void MakeSpriteVisible(GameObject thisObject, bool isVisible)     
    {
        if (isVisible)    //激活
        {
            thisObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }

        else              //隐藏
        {
            thisObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);        
        }       
    }



    private void PlayerFlip()
    {
        FacingNum = PlayerInputHandler.Instance.ProjectedMousePos.x < transform.position.x ? -1 : 1;     //如果鼠标坐标位于玩家左侧，则翻转玩家

        m_PlayerFlip.FlipX(FacingNum);
    }



    //通过鼠标滚轮拉近或拉远相机
    private void ZoomCamera()       
    {
        if (PlayerCamera != null)
        {
            float mouseScroll = PlayerInputHandler.Instance.MouseScrollInput.y;    //使用鼠标滚轮信息的y值

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

    #region Setters
    public void SetWeapon(Weapon thisWeapon, bool isPrimary)
    {
        if (thisWeapon == null)
        {
            Debug.LogError("Attempted to set a null weapon");
            return;
        }

        if (isPrimary)
        {
            PrimaryWeapon = thisWeapon;
        }
        else
        {
            SecondaryWeapon = thisWeapon;
        }
    }
    #endregion

    #region Getters
    /*
    public string GetCurrentState()
    {
        return m_CurrentState;
    }
    */
    #endregion
}
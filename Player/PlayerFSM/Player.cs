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

    //��ǰ״̬������Debug
    string m_CurrentState;
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

        MakeSpriteVisible(SecondaryWeapon.transform.gameObject, false);       //����Ϸ��ʼǰ���ظ�����

        StateMachine.Initialize(IdleState);     //��ʼ��״̬Ϊ����
    }

    private void Update()
    {
        //ʵʱ��ʾ��ǰ״̬
        m_CurrentState = StateMachine.currentState.ToString();
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
    //��������
    public async void ChangeWeapon(string weaponName, bool isPrimary)
    {
        bool isPrimaryAttackState = false;    //����������ʱ�Ƿ���Ҫ���õĹ���״̬�������Ǹ���

        if (PrimaryWeapon.transform.gameObject.GetComponent<SpriteRenderer>().color == new Color(1f, 1f, 1f, 1f))       //ͨ��͸���ȼ�鴦���ĸ�����״̬
        {
            isPrimaryAttackState = true;
        }



        if (isPrimary)
        {
            //��������֮ǰ�˳���ǰ��������״̬״̬����ֹ����Bug
            //PrimaryWeapon.ExitWeapon();

            if (PrimaryWeapon != null && PrimaryWeapon.gameObject.activeSelf)
            {
                PrimaryWeapon.gameObject.SetActive(false);      //������ǰ��ȡ�����ǰ����
            }

            await WeaponManager.Instance.LoadWeapon(weaponName, isPrimary);    //�ȴ��첽����


            PrimaryAttackState = new PlayerAttackState(this, StateMachine, PlayerData, "Idle", PrimaryWeapon);       //�����¹���״̬

            if (!isPrimaryAttackState)
            {
                MakeSpriteVisible(PrimaryWeapon.transform.gameObject, false);       //�����ǰ��������������״̬�������������ٴ�����
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
                MakeSpriteVisible(SecondaryWeapon.transform.gameObject, false);     //�����ǰ���ڸ���������״̬�������������ٴ�����
            }
        }

        //StateMachine.ChangeState(IdleState);
    }

    

    //������Ⱦ��͸�����Լ���/��������
    public void MakeSpriteVisible(GameObject thisObject, bool isVisible)     
    {
        if (isVisible)    //����
        {
            thisObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }

        else              //����
        {
            thisObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);        
        }       
    }



    private void PlayerFlip()
    {
        FacingNum = InputHandler.ProjectedMousePos.x < transform.position.x ? -1 : 1;     //����������λ�������࣬��ת���

        m_PlayerFlip.FlipX(FacingNum);
    }



    //ͨ����������������Զ���
    private void ZoomCamera()       
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack Detail")]
    public Vector2[] attackMoment;
    
    public bool isBusy {  get; private set; }
    [Header("Move info")]
    public float moveSpeed ;
    public float jumpForce ;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Jump System")]
    [SerializeField] private int maxJumpCount = 2;           // Số lần nhảy tối đa
    [SerializeField] private float doubleJumpForce;    // Lực nhảy lần 2

    [HideInInspector] public int currentJumpCount;           // Số lần nhảy hiện tại
    [HideInInspector] public bool canDoubleJump;             // Có thể double jump không
    [HideInInspector] public bool wallJumpPerformed;        // Đã thực hiện wall jump chưa

    [Header("Dash Info")]
    public float dashSpeed;
    private float defaultDashSpeed;
    public float dashDuration;
    public float dashDir {  get; private set; }

    public SkillManager skill { get; private set; }


    public bool isInDialogue { get; private set; }
    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlide {  get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }   
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this,stateMachine,"Idle");
        moveState = new PlayerMoveState(this,stateMachine,"Move");
        jumpState = new PlayerJumpState(this,stateMachine, "Jump");
        airState = new PlayerAirState(this,stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }
    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);
        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultJumpForce = doubleJumpForce;
        defaultDashSpeed = dashSpeed;

    }
    protected override void Update()
    {
        base.Update();

        // Chỉ update khi không bị block input
        if (InputManager.instance != null && !InputManager.instance.CanReceiveInput())
        {
            return;
        }

        stateMachine.currentState.Update();
        CheckForDashInput();

        // Chỉ cho phép dùng subweapon khi có thể nhận input
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (InputManager.instance.CanReceiveInput())
            {
                Inventory.instance.UseSubWeapon();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuestManager.instance.StartQuest("tutorial_kill_skeletons");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            QuestManager.instance.UpdateObjective("tutorial_kill_skeletons", "kill_skeletons", 1);
        }


    }

    public void SetDialogueState(bool inDialogue)
    {
        isInDialogue = inDialogue;

        // Cập nhật InputManager
        if (InputManager.instance != null)
        {
            InputManager.instance.SetDialogueActive(inDialogue);
        }

        if (inDialogue)
        {
            stateMachine.ChangeState(idleState);
        }
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    public IEnumerator BusyFor(float _second)
    {
        isBusy = true;
        yield return new WaitForSeconds(_second);
        isBusy = false;
    } 

    public void  AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public void CheckForDashInput()
    {
        if (IsWallDetected())   //neu phat hien tuong thi khong cho dash 
            return;

        if (skill.dash.dashUnlocked == false) // nếu skill dash chưa unlock
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0) 
                dashDir = facingDir; 
            stateMachine.ChangeState(dashState);
        }
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0,0);
    }
    #region Jump System Methods
    // Phương thức reset jump khi chạm đất
    public void ResetJumpCount()
    {
        currentJumpCount = 0;
        canDoubleJump = false;
        wallJumpPerformed = false;
    }

    // Phương thức sử dụng jump
    public bool UseJump()
    {
        AudioManager.instance.PlaySFX(42, null);
        if (currentJumpCount < maxJumpCount)
        {
            currentJumpCount++;
            return true;
        }
        return false;
    }

    // Phương thức reset jump sau wall jump (cho phép double jump)
    public void ResetJumpAfterWallJump()
    {
        currentJumpCount = 1; // Đã dùng 1 lần cho wall jump
        canDoubleJump = true; // Cho phép double jump
        wallJumpPerformed = true;
    }

    // Kiểm tra có thể double jump không
    public bool CanDoubleJump()
    {
        return canDoubleJump && currentJumpCount < maxJumpCount;
    }

    // Get jump force dựa trên lần nhảy
    public float GetJumpForce()
    {
        if (currentJumpCount == 1 && !wallJumpPerformed)
            return jumpForce; // Jump thường
        else
            return doubleJumpForce; // Double jump
    }
    #endregion
}

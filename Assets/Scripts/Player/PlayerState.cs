using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody2D rb;

    private string animBoolName;
    protected float xInput;
    protected float yInput;
    protected float stateTimer;
    protected bool triggerCalled;
    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)  //su dung dau gach ngang cho cac bien ma chung ta truyen vao qua ham     
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        //Debug.Log("I enter " + animBoolName);
        player.anim.SetBool(animBoolName,true);
        rb = player.rb;
        triggerCalled = false;

    }
    public virtual void Update()
    {

        // Kiểm tra InputManager trước khi xử lý input
        if (InputManager.instance != null && !InputManager.instance.CanReceiveInput())
        {
            player.SetZeroVelocity();
            return;
        }

        stateTimer -= Time.deltaTime;
        //Debug.Log("I'm in " + animBoolName);
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        player.anim.SetFloat("yVelocity", rb.velocity.y);

        // Kiểm tra jump input
        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.ChangeState(player.jumpState);
    }

    public virtual void Exit()
    {
        //Debug.Log("I exit " + animBoolName);
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}

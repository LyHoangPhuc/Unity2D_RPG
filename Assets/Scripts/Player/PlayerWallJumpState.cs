using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Reset jump count sau wall jump - cho phép double jump
        player.ResetJumpAfterWallJump();

        // Wall jump logic (giữ nguyên code cũ)
        rb.velocity = new Vector2(5 * -player.facingDir, player.jumpForce);

        // Audio feedback cho wall jump
        //AudioManager.instance.PlaySFX(3, player.transform);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0) 
            stateMachine.ChangeState(player.airState);

        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);
    }
}

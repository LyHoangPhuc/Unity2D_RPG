using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Reset jump count khi chạm đất
        player.ResetJumpCount();
        // Reset horizontal velocity nếu không có input
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f)
        {
            player.SetVelocity(0, player.rb.velocity.y);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // Chỉ di chuyển khi có input
        if (Input.GetAxisRaw("Horizontal") != 0 && !player.isBusy)
            stateMachine.ChangeState(player.moveState);

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.J)) 
            stateMachine.ChangeState(player.primaryAttack);

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);
    
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);
    }
}

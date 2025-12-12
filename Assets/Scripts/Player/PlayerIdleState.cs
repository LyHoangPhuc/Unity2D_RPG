using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //rb.velocity = new Vector2(0, 0);
        // Reset jump count khi về idle (đảm bảo reset)
        player.ResetJumpCount();
        // Dừng movement hoàn toàn
        player.SetVelocity(0, player.rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // Chỉ chuyển sang move khi có input
        if (Input.GetAxisRaw("Horizontal") != 0 && !player.isBusy)
            stateMachine.ChangeState(player.moveState);

        if (xInput == player.facingDir && player.IsWallDetected())
            return;

        if (xInput != 0 && !player.isBusy)
            stateMachine.ChangeState(player.moveState);
    }
}

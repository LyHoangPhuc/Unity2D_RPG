using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(39,null);
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSFX(39);
    }

    public override void Update()
    {
        base.Update();

        // Lấy input hiện tại
        float currentInput = Input.GetAxisRaw("Horizontal");

        if (currentInput == 0 || player.isBusy)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if (currentInput != 0)
        {
            player.SetVelocity(currentInput * player.moveSpeed, rb.velocity.y);
        }

        //player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        //if (xInput == 0 || player.IsWallDetected())
        //    stateMachine.ChangeState(player.idleState);
    }
}

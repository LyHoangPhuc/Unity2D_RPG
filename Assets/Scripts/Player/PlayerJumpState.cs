using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Sử dụng jump count
        if (player.UseJump())
        {
            // Reset velocity Y
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            // Apply jump force dựa trên loại jump
            rb.velocity = new Vector2(rb.velocity.x, player.GetJumpForce());

            // Audio feedback khác nhau cho jump thường và double jump
            //if (player.currentJumpCount == 1)
            //    AudioManager.instance.PlaySFX(3, player.transform); // Jump sound
            //else
            //    AudioManager.instance.PlaySFX(4, player.transform); // Double jump sound
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();


        if (rb.velocity.y < 0)
            stateMachine.ChangeState(player.airState);
    }
}

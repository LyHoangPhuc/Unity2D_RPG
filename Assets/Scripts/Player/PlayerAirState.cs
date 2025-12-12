using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.StopSFX(42);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // Kiểm tra wall slide
        if (player.IsWallDetected() && xInput == player.facingDir && player.rb.velocity.y <= 0)
            stateMachine.ChangeState(player.wallSlide);

        // Double Jump Logic - THÊM MỚI
        if (Input.GetKeyDown(KeyCode.Space) && player.CanDoubleJump())
        {
            // Thực hiện double jump
            if (player.UseJump())
            {
                rb.velocity = new Vector2(rb.velocity.x, player.GetJumpForce());

                // Audio và visual feedback cho double jump
                //AudioManager.instance.PlaySFX(4, player.transform); // Double jump sound

            }
        }

        // Kiểm tra chạm đất TRƯỚC khi xử lý movement
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
            return; // QUAN TRỌNG: Thoát khỏi method để không xử lý movement
        }

        // Movement trong không khí - CHỈ KHI CÓ INPUT
        if (xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);
        }
        else
        {
            // Nếu không có input, giảm dần velocity X (air friction)
            player.SetVelocity(rb.velocity.x * 0.95f, rb.velocity.y);
        }
    }
}

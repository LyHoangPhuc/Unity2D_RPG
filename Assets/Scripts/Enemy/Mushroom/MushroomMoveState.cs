using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomMoveState : MushroomGroundedState
{
    public MushroomMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBollName, Enemy_Mushroom enemy) : base(_enemyBase, _stateMachine, _animBollName, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);

        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }

        if (enemy.IsPlayerDetected())
            stateMachine.ChangeState(enemy.battleState);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomGroundedState : EnemyState
{
    protected Enemy_Mushroom enemy;
    protected Transform player;

    public MushroomGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBollName, Enemy_Mushroom _enemy) : base(_enemyBase, _stateMachine, _animBollName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.transform.position) < 2)
            stateMachine.ChangeState(enemy.battleState);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomAttackState : EnemyState
{
    private Enemy_Mushroom enemy;

    public MushroomAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBollName, Enemy_Mushroom _enemy) : base(_enemyBase, _stateMachine, _animBollName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(2, enemy.transform);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}

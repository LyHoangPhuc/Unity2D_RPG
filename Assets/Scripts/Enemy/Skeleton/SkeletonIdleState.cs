using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonIdleState : SkeletonGroundedState
{
    public SkeletonIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBollName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBollName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        AudioManager.instance.PlaySFX(38, enemy.transform); 
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0) 
            stateMachine.ChangeState(enemy.moveState);
    }
}

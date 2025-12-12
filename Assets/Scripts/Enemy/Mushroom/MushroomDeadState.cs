using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomDeadState : EnemyState
{
    private Enemy_Mushroom enemy;
    public MushroomDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBollName, Enemy_Mushroom _enemy) : base(_enemyBase, _stateMachine, _animBollName)
    {
        this.enemy = _enemy;
    }



    public override void Enter()
    {
        base.Enter();
        enemy.anim.SetBool(enemy.lastAnimBoolName, true);
        enemy.anim.speed = 0;
        enemy.cd.enabled = false;
        stateTimer = .1f;
    }



    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 10);
    }
}

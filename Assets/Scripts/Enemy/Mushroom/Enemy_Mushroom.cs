using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public class Enemy_Mushroom : Enemy
{
    #region States
    public MushroomIdleState idleState { get; private set; }
    public MushroomMoveState moveState { get; private set; }
    public MushroomBattleState battleState { get; private set; }
    public MushroomAttackState attackState { get; private set; }
    public MushroomDeadState deadState { get; private set; }
    public MushroomStunnedState stunnedState { get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();

        idleState = new MushroomIdleState(this, stateMachine, "Idle", this);
        moveState = new MushroomMoveState(this, stateMachine, "Move", this);
        battleState = new MushroomBattleState(this, stateMachine, "Move", this);
        attackState = new MushroomAttackState(this, stateMachine, "Attack", this);
        deadState = new MushroomDeadState(this, stateMachine, "Idle", this);
        stunnedState = new MushroomStunnedState(this, stateMachine, "Stunned", this);

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.U))
        {
            stateMachine.ChangeState(stunnedState);
        }
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }
}

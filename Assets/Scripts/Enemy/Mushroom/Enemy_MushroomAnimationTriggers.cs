using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_MushroomAnimationTriggers : MonoBehaviour
{
    private Enemy_Mushroom enemy => GetComponentInParent<Enemy_Mushroom>();

    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        foreach (var hit in collider)
        {
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(target);
            }
            //hit.GetComponent<Player>().Damage();
        }
    }
}

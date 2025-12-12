using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        AudioManager.instance.PlaySFX(36, null);

        // TH�M PARTICLE EFFECT
        if (ParticleManager.instance != null)
        {
            Vector3 effectPos = player.attackCheck.position;
            ParticleManager.instance.PlayAttackEffect(effectPos, player.FacingRight);
        }
        //

        Collider2D[] collider = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius); //Physics2D.OverlapCircleAll: h�m cua unity, kiem tra xem co bat ki doi tuong co Collider2D nam trong vung tron nhat dinh hay khong
                                                                                                                   //tra ve 1 mang cac doi tuong kieu Collider2D  nam trong vung kiem tra
        foreach (var hit in collider)   //duyet qua tung doi tuong trong mang collider
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //kiem tra xem doi tuong hien tai (hit) co chua component Enemy hay khong (de xac dinh co phai la ke thu hay khong)
                    
                EnemyStats _target = hit.GetComponent<EnemyStats>();
                player.stats.DoDamage(_target);

                //lay vu khi tu kho de goi hieu ung vat pham
                ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);
                if (weaponData != null)
                    weaponData.Effect(_target.transform);
                //Inventory.instance.GetEquipment(EquipmentType.Weapon).Effect(_target.transform);

            }

            // THÊM PHẦN NÀY: Kiểm tra destructible objects
            if (hit.GetComponent<DestructibleObject>() != null)
            {
                DestructibleObject destructible = hit.GetComponent<DestructibleObject>();
                int playerDamage = player.stats.damage.GetValue() + player.stats.strength.GetValue();
                destructible.OnPlayerAttack(playerDamage);
            }
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player; 
    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if(_damage > GetMaxHealthValue() * .3f) 
        {
            player.SetupKnockbackPower(new Vector2(10, 6));
            //Debug.Log("High dame taken");
        }

        //ItemData_Equipment currenArmor = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        //ItemData_Equipment currenArmor = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        //ItemData_Equipment currenArmor = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        //ItemData_Equipment currenArmor = Inventory.instance.GetEquipment(EquipmentType.Amulet);
          
    }
}

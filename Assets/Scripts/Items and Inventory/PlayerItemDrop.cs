using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;
    public override void GenerateDrop()
    {
        //list of equipment
        Inventory inventory = Inventory.instance;

        //List<InventoryItem> currentStash = inventory.GetStashList();
        //List<InventoryItem> currentEquipment = inventory.GetEquipmentList();
        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialToLoose = new List<InventoryItem>();

        //foreach item we gonna check if should loose item
        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            if(Random.Range(0,100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
                //inventory.UnequipItem(item.data as ItemData_Equipment);
            }
        }

        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        }

        foreach (InventoryItem item in inventory.GetStashList())
        {
            if(Random.Range(0,100) <= chanceToLooseMaterials)
            {
                DropItem(item.data);
                materialToLoose.Add(item);
            }
        }

        for (int i = 0; i < materialToLoose.Count; i++)
        {
            inventory.RemoveItem(materialToLoose[i].data);
        }
    }
}

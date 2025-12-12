using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possiableDrop;
    private List<ItemData> droplist = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;


    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possiableDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possiableDrop[i].dropChange)
                droplist.Add(possiableDrop[i]);
        }

        if (droplist.Count == 0)
            return;


        //for (int i = 0; i < possibleItemDrop; i++)
        //{
        //    ItemData randomItem = droplist[Random.Range(0, droplist.Count - 1)];

        //    droplist.Remove(randomItem);
        //    DropItem(randomItem);
        //}

        int itemsToDrop = Mathf.Min(possibleItemDrop, droplist.Count);

        for (int i = 0; i < itemsToDrop; i++)
        {
            int randomIndex = Random.Range(0, droplist.Count);
            ItemData randomItem = droplist[randomIndex];
            droplist.RemoveAt(randomIndex);
            DropItem(randomItem);
        }
    }



    protected void DropItem(ItemData _itemdata)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemdata, randomVelocity);
    }

}

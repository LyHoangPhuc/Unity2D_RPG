using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;
    public InventoryItem item;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }
    public void UpdateSlot(InventoryItem _newitem)
    {
        //Debug.Log($"[ItemSlot] Updating slot with item: {(_newitem?.data?.itemName ?? "null")}");
        item = _newitem;

        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;
            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
    }

    public void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }
    public virtual void OnPointerDown(PointerEventData eventData)  //Dùng khi click chuột xuống trên các đối tượng có trong giao diện(UI) này
    {
        //Debug.Log($"[ItemSlot] Clicked on item: {(item?.data?.itemName ?? "empty slot")}");
        //Debug.Log("OnPointerDown được gọi trên " + gameObject.name);

        if (item == null)
        {
            //Debug.Log("Item là null, không thực hiện hành động");
            return;
        }
        //if (item == null )
        //    return;

        if (Input.GetKey(KeyCode.Q))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }
        if(item.data.itemType == ItemType.Equipment) 
        {
            Inventory.instance.EquipItem(item.data);
        }

        ui.itemTooltip.HideToolTip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.itemTooltip.ShowToolTip(item.data as ItemData_Equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.itemTooltip.HideToolTip();
    }
}

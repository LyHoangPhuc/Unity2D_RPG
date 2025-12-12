using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

 public enum EquipmentType
{
    Weapon,
    SubWeapon,
    Amulet,
    Helmet,
    Chestplate,
    Leggings,
    Boots
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")] //tạo tên trong menu để tạo Data -> equipment 
public class ItemData_Equipment : ItemData //kế thừa itemData
{
    public EquipmentType equipmentType;

    [Header ("Unique effect")]
    public float itemCoolDown;
    public ItemEffect[] itemEffects;

    [Header("Major stats")]
    public int strength;
    public int agility;
    public int intelligence;
    public int vitality;

    [Header("Offensive stats")]
    public int damage;
    public int critRate;
    public int critDame;

    [Header("Defensive stats")]
    public int health;
    public int armor;
    public int evasion;
    public int magicResistance;

    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightingDamage;

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;

    private int descriptionLength;
    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        // Lưu tỷ lệ máu hiện tại
        float healthPercentage = (float)playerStats.currentHealth / playerStats.GetMaxHealthValue();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critRate.AddModifier(critRate);
        playerStats.critDame.AddModifier(critDame);

        // Cập nhật maxHealth trước
        playerStats.maxHealth.AddModifier(health);

        // Cập nhật currentHealth theo tỷ lệ (hoặc full health)
        if (health > 0) // Nếu item tăng máu
        {
            playerStats.currentHealth += health;

            // Đảm bảo không vượt quá maxHealth
            if (playerStats.currentHealth > playerStats.GetMaxHealthValue())
                playerStats.currentHealth = playerStats.GetMaxHealthValue();

            // Trigger event để cập nhật UI
            if (playerStats.onHealthChanged != null)
                playerStats.onHealthChanged();
        }

            playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamage.AddModifier(lightingDamage);
    }

    public void RemoveModifier()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        // Lưu tỷ lệ máu hiện tại
        float healthPercentage = (float)playerStats.currentHealth / playerStats.GetMaxHealthValue();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critRate.RemoveModifier(critRate);
        playerStats.critDame.RemoveModifier(critDame);

        playerStats.maxHealth.RemoveModifier(health);
        // Cập nhật currentHealth
        if (health > 0)
        {
            // Giữ nguyên tỷ lệ máu
            playerStats.currentHealth = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healthPercentage);

            // Đảm bảo không vượt quá maxHealth mới
            if (playerStats.currentHealth > playerStats.GetMaxHealthValue())
                playerStats.currentHealth = playerStats.GetMaxHealthValue();

            // Trigger event để cập nhật UI
            if (playerStats.onHealthChanged != null)
                playerStats.onHealthChanged();
        }

        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightingDamage.RemoveModifier(lightingDamage);
    }


    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "Strength");
        AddItemDescription(agility, "Agility");
        AddItemDescription(intelligence, "Intelligence");
        AddItemDescription(vitality, "Vitality");

        AddItemDescription(damage, "Damage");
        AddItemDescription(critRate, "CritRate");
        AddItemDescription(critDame, "CritDame");

        AddItemDescription(health, "Health");
        AddItemDescription(armor, "Armor");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(magicResistance, "MagicResistance");

        AddItemDescription(fireDamage, "FireDamage");
        AddItemDescription(iceDamage, "IceDamage");
        AddItemDescription(lightingDamage, "LightingDamage");



        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i].effectDescription.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Unique: " + itemEffects[i].effectDescription);
                descriptionLength++;
            }
        }


        if (descriptionLength < 5)
        {
            for (int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        return sb.ToString();
    }

    private void AddItemDescription(int _value,string _name)
    {
        if(_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();
            if (_value > 0)
                sb.Append("+ " + _value + " " + _name);

            descriptionLength++;
        }
    }
}

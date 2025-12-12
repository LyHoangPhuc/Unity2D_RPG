using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;
    // Start is called before the first frame update

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if (statNameText != null)
        {
            statNameText.text = statName;
        }
    }
    void Start()
    {
        UpdateStatValueUI();

        ui = GetComponentInParent<UI>();
    }

    // Update is called once per frame
    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();


            if (statType == StatType.health)
                statValueText.text = playerStats.GetMaxHealthValue().ToString();

            //1 point strength = 1 point crit damage + 1 point damage
            if (statType == StatType.damage)
                statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();

            if (statType == StatType.critDame)
                statValueText.text = (playerStats.critDame.GetValue() + playerStats.strength.GetValue()).ToString();

            //1 point agility  = 1 point crit rate & 1 point evasion 
            if (statType == StatType.critRate)
                statValueText.text = (playerStats.critRate.GetValue() + playerStats.agility.GetValue()).ToString();

            if (statType == StatType.evasion)
                statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();

            // 1 point intelligence = 1 point magic damage & 3 point magic resistance 
            if (statType == StatType.magicResistance)
                statValueText.text = (playerStats.magicResistance.GetValue() + (playerStats.intelligence.GetValue() *3)).ToString();


        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statTooltip.ShowStatToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statTooltip.HideStatToolTip();
    }
}

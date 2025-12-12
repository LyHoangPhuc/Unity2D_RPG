using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillTooltip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillCost;
    [SerializeField] private float defaultNameFontSize;

    public void ShowToolTip(string _skillDescription, string _skillName, int _price)
    {
        skillText.text = _skillDescription;
        skillName.text = _skillName;
        skillCost.text = "Cost: " + _price;

        AdjustPosition();
        AdjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    // Update is called once per frame
    public void HideTooltip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    } 

}

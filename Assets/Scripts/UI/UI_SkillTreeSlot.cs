using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler, ISaveManager
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillCost;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    public bool unlocked;

    [Header("Skill Prerequisites")]
    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        skillImage.color = lockedSkillColor;

        if (unlocked)
            skillImage.color = Color.white;
    }
    public void UnlockSkillSlot()
    {
        if (!CanUnlockSkill())
            return;

        if (PlayerManager.instance.HaveEnoughMoney(skillCost) == false)
            return;

        unlocked = true;
        skillImage.color = Color.white;
    }

    public bool CanUnlockSkill()
    {
        // Check if already unlocked
        if (unlocked)
            return false;

        // Check prerequisites
        foreach (UI_SkillTreeSlot skillSlot in shouldBeUnlocked)
        {
            if (skillSlot.unlocked == false)
                return false;
        }

        // Check locked requirements
        foreach (UI_SkillTreeSlot skillSlot in shouldBeLocked)
        {
            if (skillSlot.unlocked == true)
                return false;
        }

        return true;
    }

    //public void UnlockSkillSlot()
    //{
    //    if (PlayerManager.instance.HaveEnoughMoney(skillCost) == false)
    //        return;
    //    Debug.Log("Slot unlock");
    //    for (int i = 0; i < shouldBeUnlocked.Length; i++)
    //    {
    //        if (shouldBeUnlocked[i].unlocked == false)
    //        {
    //            Debug.Log("Cannot unlock skill");
    //            return;
    //        }
    //    }
    //    for (int i = 0; i < shouldBeLocked.Length; i++)
    //    {
    //        if (shouldBeLocked[i].unlocked == true)
    //        {
    //            Debug.Log("Cannot unlock skill");
    //            return;
    //        }
    //    }
    //    unlocked = true;
    //    skillImage.color = Color.white;
    //}
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowToolTip(skillDescription,skillName,skillCost);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideTooltip();
    }

    public void LoadData(GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;

            if (skillImage == null)
                skillImage = GetComponent<Image>();

            if (unlocked)
                skillImage.color = Color.white;
            else
                skillImage.color = lockedSkillColor;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if (_data.skillTree.ContainsKey(skillName))
            _data.skillTree.Remove(skillName);

        _data.skillTree.Add(skillName, unlocked);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image subWeaponImage;

    private SkillManager skills;

    [Header("Soul info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulAmount;
    [SerializeField] private float increaseRate = 100;

    [Header("Level System")]
    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private GameObject levelUpEffect;
    [SerializeField] private TextMeshProUGUI levelUpNewLevel;
    [SerializeField] private TextMeshProUGUI levelUpStats;
    [SerializeField] private Animator levelUpAnimator;

    private Coroutine soulUpdateCoroutine;
    private WaitForSeconds updateInterval = new WaitForSeconds(0.1f); // COMMENT: Update mỗi 0.1s thay vì mỗi frame
    [SerializeField] private TextMeshProUGUI expNumbers;
    [SerializeField] private Slider expSlider;



    void Start()
    {
        if(playerStats != null)
        {
            playerStats.onHealthChanged += UpdateHealthUI;
        }

        skills = SkillManager.instance;

        // Subscribe to level events
        if (PlayerLevel.instance != null)
        {
            PlayerLevel.instance.onLevelUp += OnLevelUp;
            PlayerLevel.instance.onExpGained += UpdateLevelUI;
            PlayerLevel.instance.onDataLoaded += UpdateLevelUI;
        }

        // Chỉ update UI nếu data đã sẵn sàng
        if (PlayerLevel.instance != null && PlayerLevel.instance.currentLevel > 1)
        {
            UpdateLevelUI();
        }
       
        soulUpdateCoroutine = StartCoroutine(UpdateSoulsCoroutine());
    }


    // Update is called once per frame
    void Update()
    {
        UpdateSoulsUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
            SetCooldownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.SubWeapon) != null)
            SetCooldownOf(subWeaponImage);


        CheckCooldownOf(dashImage, skills.dash.cooldown);
        CheckCooldownOf(subWeaponImage, Inventory.instance.subWeaponCoolDown);
    }

    //void Update()
    //{
    //    // COMMENT: Chỉ giữ lại input handling, remove expensive operations
    //    if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
    //        SetCooldownOf(dashImage);

    //    if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.SubWeapon) != null)
    //        SetCooldownOf(subWeaponImage);
    //}

    private IEnumerator UpdateSoulsCoroutine()
    {
        // COMMENT: Coroutine để update souls và cooldowns
        while (true)
        {
            UpdateSoulsUI();
            CheckCooldownOf(dashImage, skills.dash.cooldown);
            CheckCooldownOf(subWeaponImage, Inventory.instance.subWeaponCoolDown);
            yield return updateInterval;
        }
    }

    private void UpdateSoulsUI()
    {
        if (soulAmount < PlayerManager.instance.GetCurrency())
            soulAmount += Time.deltaTime * increaseRate;
        else
            soulAmount = PlayerManager.instance.GetCurrency();


        currentSouls.text = ((int)soulAmount).ToString();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void UpdateLevelUI()
    {
        if (PlayerLevel.instance == null) return;

        levelNumber.text = PlayerLevel.instance.currentLevel.ToString();
        expNumbers.text = PlayerLevel.instance.currentExp + " / " + PlayerLevel.instance.expToNextLevel;
        expSlider.value = PlayerLevel.instance.GetExpPercentage();
    }

    private void CheckCooldownOf(Image _image , float _coolDown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _coolDown * Time.deltaTime;
    }
    
    private void OnDestroy()
    {
        if (PlayerLevel.instance != null)
        {
            PlayerLevel.instance.onLevelUp -= OnLevelUp;
            PlayerLevel.instance.onExpGained -= UpdateLevelUI;
        }
    }

    public void OnLevelUp()
    {

        // Update level up effect text
        levelUpNewLevel.text = "Level " + PlayerLevel.instance.currentLevel;
        levelUpStats.text = "+" + PlayerLevel.instance.HealthPerLevel + " Health" + " +" + PlayerLevel.instance.DamagePerLevel + " Damage " + "+" + PlayerLevel.instance.ArmorPerLevel + " Armor";

        // Show animation
        if (levelUpAnimator != null &&
            levelUpAnimator.runtimeAnimatorController != null)
        {
            StartCoroutine(ShowLevelUpEffect());
            levelUpAnimator.SetTrigger("ShowLevelUp");
        }
        else
        {
            Debug.LogWarning("Level Up Animator is not properly configured!");
        }

        UpdateLevelUI();
    }

    private IEnumerator ShowLevelUpEffect()
    {
        levelUpEffect.SetActive(true);
        yield return new WaitForSeconds(2f);
        levelUpEffect.SetActive(false);
    }
}

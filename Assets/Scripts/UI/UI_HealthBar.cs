using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private CharacterStats myStats => GetComponentInParent<CharacterStats>();
    private EnemyStats enemyStats => GetComponentInParent<EnemyStats>();
    private RectTransform myTransform;
    private Slider slider;

    [Header("Level Display")]
    [SerializeField] private TextMeshProUGUI info; // Text để hiển thị level   
    [SerializeField] private bool showLevel = true;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

        UpdateHealthUI();
        UpdateInfoDisplay();
    }


    private void UpdateHealthUI()
    {
        // Kiểm tra null trước khi update
        if (this == null || gameObject == null)
            return;

        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void UpdateInfoDisplay()
    {
        // Chỉ hiển thị level cho enemy
        if (info != null && enemyStats != null && showLevel)
        {
            info.text = GetEnemyName() + " Lv." + enemyStats.GetLevel().ToString();
            info.gameObject.SetActive(true);
        }
        else if (info != null)
        {
            // Ẩn info player hoặc khi không muốn hiển thị
            info.gameObject.SetActive(false);
        }
    }
    private string GetEnemyName()
    {
        // Lấy tên từ GameObject name và format lại
        string rawName = transform.root.name;

        // Xử lý tên để hiển thị đẹp hơn
        if (rawName.Contains("Enemy_"))
        {
            rawName = rawName.Replace("Enemy_", "");
        }

        return rawName;
    }

        private void FlipUI()
    {
        myTransform.Rotate(0, 180, 0);
    }

    private void OnEnable()
    {
        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        if(entity != null)
            entity.onFlipped -= FlipUI;

        if(myStats != null)
            myStats.onHealthChanged -= UpdateHealthUI;
    }

    private Coroutine healthUpdateCoroutine;

    private void OnDestroy()
    {
        // Stop all coroutines khi object bị destroy
        if (healthUpdateCoroutine != null)
        {
            StopCoroutine(healthUpdateCoroutine);
        }
        StopAllCoroutines();
    }

}

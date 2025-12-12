using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerLevel : MonoBehaviour, ISaveManager
{
    public static PlayerLevel instance;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("Level Scaling")]
    [SerializeField] private float expMultiplier = 1.2f;
    [SerializeField] private int baseExpRequirement = 100;

    [Header("Stat Bonuses per Level")]
    [SerializeField] private int healthPerLevel;
    [SerializeField] private int damagePerLevel;
    [SerializeField] private int armorPerLevel;

    public System.Action onLevelUp;
    public System.Action onExpGained;

    private PlayerStats playerStats;
    private GameData pendingLoadData; // Lưu trữ data cần load
    private bool isInitialized = false;


    public int HealthPerLevel => healthPerLevel;
    public int DamagePerLevel => damagePerLevel;
    public int ArmorPerLevel => armorPerLevel;

    public System.Action onDataLoaded;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // ✅ THÊM DÒNG NÀY
        RefreshPlayerStatsReference();

        // Đợi PlayerManager khởi tạo xong
        StartCoroutine(InitializePlayerLevel());
    }
    // ✅ THÊM METHOD NÀY
    public void RefreshPlayerStatsReference()
    {
        if (PlayerManager.instance != null && PlayerManager.instance.player != null)
        {
            playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            Debug.Log("PlayerLevel: PlayerStats reference refreshed");
        }
        else
        {
            Debug.LogWarning("PlayerLevel: Cannot refresh PlayerStats - PlayerManager or Player is null");
        }
    }

    private IEnumerator InitializePlayerLevel()
    {
        // Đợi cho đến khi PlayerManager và player sẵn sàng
        while (PlayerManager.instance == null || PlayerManager.instance.player == null)
        {
            yield return null; // Đợi 1 frame
        }

        // Bây giờ mới an toàn để gán
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        isInitialized = true;

        CalculateExpRequirement();

        // Nếu có pending load data, thực hiện load ngay
        if (pendingLoadData != null)
        {
            LoadDataInternal(pendingLoadData);
            pendingLoadData = null;
        }

    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        onExpGained?.Invoke();

        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        AudioManager.instance.PlaySFX(44, null);
        currentExp -= expToNextLevel;
        currentLevel++;

        // Refresh reference trước khi sử dụng
        if (playerStats == null)
        {
            RefreshPlayerStatsReference();
        }

        if (playerStats != null)
        {
            playerStats.IncreaseHealthBy(playerStats.GetMaxHealthValue());
        }
        // Apply stat bonuses
        ApplyLevelBonuses();

        // Calculate new exp requirement
        CalculateExpRequirement();

        // Heal player on level up (chỉ khi playerStats đã sẵn sàng)
        if (playerStats != null)
        {
            playerStats.IncreaseHealthBy(playerStats.GetMaxHealthValue());
        }

        onLevelUp?.Invoke();

        Debug.Log($"Level Up! New Level: {currentLevel}");

        // Quest tracking
        QuestManager.instance.UpdateObjective("level_up", "player_level", 1);
    }

    private void ApplyLevelBonuses()
    {
        // Kiểm tra và refresh nếu cần
        if (playerStats == null)
        {
            RefreshPlayerStatsReference();
        }

        // Kiểm tra playerStats trước khi sử dụng
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats is null, cannot apply level bonuses");
            return;
        }

        playerStats.maxHealth.AddModifier(healthPerLevel);
        playerStats.damage.AddModifier(damagePerLevel);
        playerStats.armor.AddModifier(armorPerLevel);
    }

    private void CalculateExpRequirement()
    {
        expToNextLevel = Mathf.RoundToInt(baseExpRequirement * Mathf.Pow(expMultiplier, currentLevel - 1));
    }

    public float GetExpPercentage()
    {
        return (float)currentExp / expToNextLevel;
    }

    public void LoadData(GameData _data)
    {
        if (!isInitialized)
        {
            // Nếu chưa khởi tạo xong, lưu data để load sau
            pendingLoadData = _data;
            return;
        }

        LoadDataInternal(_data);
    }

    private void LoadDataInternal(GameData _data)
    {
        currentLevel = _data.playerLevel;
        currentExp = _data.playerExp;
        CalculateExpRequirement();


        // Apply all level bonuses (chỉ khi playerStats đã sẵn sàng)
        if (playerStats != null)
        {
            for (int i = 1; i < currentLevel; i++)
            {
                ApplyLevelBonuses();
            }
        }
        // Thông báo rằng data đã được load xong
        onDataLoaded?.Invoke();
    }
    public void SaveData(ref GameData _data)
    {
        _data.playerLevel = currentLevel;
        _data.playerExp = currentExp;
    }
}

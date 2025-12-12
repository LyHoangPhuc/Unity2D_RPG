using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stat soulsDropAmount;

    [Header("Enemy Info")]
    public string enemyId; // Unique ID for quest tracking
    public string enemyName;
    public EnemyType enemyType = EnemyType.Undead;

    [Header("Quest Integration")]
    [SerializeField] private bool trackForQuests = true;
    [SerializeField] private string[] questTags; // Additional tags for specific quests

    // Events for quest tracking
    public System.Action<EnemyStats> OnEnemyKilled;


    [Header("Level details")]
    [SerializeField] private int level = 1;

    [SerializeField] private float percentageModifier = .4f;
    [Range(0f, 1f)]

    [Header("Experience Reward")]
    public int expReward = 10;

    public int GetLevel()
    {
        return level;
    }

    protected override void Start()
    {
        //soulsDropAmount.SetDefaultValue(5);
        ApplyLevelModifiers();

        base.Start();
        enemy = GetComponent<Enemy>();

        myDropSystem = GetComponent<ItemDrop>();

    }

    private void ApplyLevelModifiers()
    {
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);
        Modify(strength);

        Modify(damage);
        Modify(critRate);
        Modify(critDame);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);

        Modify(soulsDropAmount);
    }

    private void Modify(Stat _stat)
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = _stat.GetValue() * percentageModifier;
            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        // Track for quests before dying
        if (trackForQuests)
        {
            TrackEnemyKill();
        }

        // Trigger death event for quest system
        OnEnemyKilled?.Invoke(this);

        enemy.Die();

        

        PlayerManager.instance.currency += soulsDropAmount.GetValue();

        // Th�m exp reward
        int expReward = Mathf.RoundToInt(level * 10);
        if (PlayerLevel.instance != null)
        {
            PlayerLevel.instance.GainExp(expReward);
        }

        myDropSystem.GenerateDrop();
        Destroy(gameObject, .5f);
    }

    private void TrackEnemyKill()
    {
        if (QuestManager.instance == null) return;

        // Track by enemy ID (skeleton, mushroom, etc.)
        QuestManager.instance.UpdateObjectiveProgress("kill", enemyId, 1);

        // Track by enemy type (undead, normal, etc.)
        QuestManager.instance.UpdateObjectiveProgress("kill", enemyType.ToString().ToLower(), 1);

        // Track by quest tags if any
        if (questTags != null)
        {
            foreach (string tag in questTags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    QuestManager.instance.UpdateObjectiveProgress("kill", tag, 1);
                }
            }
        }

        // Generic enemy kill tracking
        QuestManager.instance.UpdateObjectiveProgress("kill", "any", 1);

        Debug.Log($"Quest tracking: Enemy {enemyId} killed");
    }

    // Public methods for quest system
    public string GetEnemyId() => enemyId;
    public EnemyType GetEnemyType() => enemyType;
    public string[] GetQuestTags() => questTags;

    [ContextMenu("Debug: Kill This Enemy")]
    private void DebugKillEnemy()
    {
        Die();
    }
}
public enum EnemyType
{
    Normal,
    Elite,
    Boss,
    Undead,
    Beast,
    Demon,
    Elemental
}



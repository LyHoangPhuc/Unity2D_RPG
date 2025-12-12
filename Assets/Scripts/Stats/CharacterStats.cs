using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critRate,
    critDame,
    health,
    armor,
    evasion,
    magicResistance,
    fireDamage,
    iceDamage,
    lightingDamage
}
public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;
    [Header("Major stats")]
    public Stat agility;  //1 point  = 1% crit rate & 1% evasion 
    public Stat intelligence; // 1 point = 1% magic damage & 3% magic resistance 
    public Stat vitality; //1 point = 5 point health 
    public Stat strength; //1 point = 1% crit damage + 1% damage

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critRate;
    public Stat critDame; //default 150

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    
    public bool isIgnited;   //gay sat thuong theo thoi gian
    public bool isChilled;   //giam giap 20%
    public bool isShocked;   //giam ti le danh trung 20%

    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float ignitedDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

    public int currentHealth;

    public System.Action onHealthChanged;
    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }

    protected virtual void Start()
    {
        //critDame.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();

    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if(isIgnited)
            ApplyIgniteDamage();
    }
    #region Buff_Stats
    public virtual void IncreaseStatBy(int _modifier,float _duration,Stat _statToModify)
    {
        //start corototuine dor stats increase
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }
    #endregion

    #region Damage_Event
    public virtual void DoDamage(CharacterStats _targetStats)
    {

        if (_targetStats.isInvincible)
            return;

        if (TargetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            //Debug.Log("Total crit damage is " + totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        //neu thanh kiem co hieu ung lua thi moi thuc hien ham nay
        //remove if you dont want to apply magic hit on primary attack
        DoMagicalDamage(_targetStats);
    }
    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible)
            return;

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact(); 
        fx.StartCoroutine("FlashFX");


        //Debug.Log(_damage);

        if (currentHealth < 0 && !isDead)
            Die();
    }
    #endregion

    #region Magical damage and ailments
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        
        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage);


        //ham MaxOfThree de kiem tra dieu kien trc khi vao vong while ben duoi, neu nhu khong co MaxOfThree thi se khong thoat duoc vong while, khien Unity bi treo
        float MaxOfThree(float a, float b, float c)
        {
            return MathF.Max(MathF.Max(a, b), c);
        }

        if (MaxOfThree(_fireDamage, _iceDamage, _lightingDamage) <= 0)
        {
            return;
        }

        

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while(!canApplyIgnite && !canApplyChill && !canApplyShock)   //neu khong the ap duoc hieu ung thi tien hanh random 50:50
        {
            if(UnityEngine.Random.value < .3f && _fireDamage > 0)  //cho hieu ung lua la 30:70 vi hieu ung lua co luot di dau tien
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Applied fire");
                return;
            }

            if (UnityEngine.Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Applied ice");
                return;
            }

            if (UnityEngine.Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Applied lighting");
                return;
            }
        }

        if(canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);


    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if(_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFxFor(ailmentsDuration);
        }

        if(_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }

        if (_shock && canApplyShock)
        {
            if(!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if(GetComponent<Player>() != null)  //neu doi tuong co component cua nguoi choi thi HitNearestTargetWithShockStrike khong hoat dong
                    return;                         
                HitNearestTargetWithShockStrike();  

            }

        }


    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        isShocked = _shock;
        shockedTimer = ailmentsDuration;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)        
                closestEnemy = transform;
        }
        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            Debug.Log("Take burn damage" + igniteDamage);
            DecreaseHealthBy(igniteDamage);
            if (currentHealth < 0 && !isDead)
                Die();
            igniteDamageTimer = ignitedDamageCooldown;
        }
    }
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion

    #region Stats caculation#
    private static int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if(_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (UnityEngine.Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }
        return false;
    }

    // Update is called once per frame
    
    private bool CanCrit()
    {
        int totalCriticalChance = critRate.GetValue() + agility.GetValue();
        if(UnityEngine.Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalcritDame = (critDame.GetValue() + strength.GetValue()) * .01f;
        //Debug.Log("total crit  power %" + totalcritDame);

        float critDamage = _damage * totalcritDame;
        //Debug.Log("crit damage before round up" + critDamage);

        return Mathf.RoundToInt(critDamage);
    }


    #endregion

    #region Change current HP
    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;
        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;

        //Debug.Log(_damage);
        //if (_damage <= 0)
        //    return;

        //if (_damage > 0)
        fx.CreatePopUpText(_damage.ToString());

        if (onHealthChanged != null)
            onHealthChanged();

    }
    public virtual void UpdateCurrentHealthOnMaxHealthChange(int oldMaxHealth, int newMaxHealth)
    {
        if (newMaxHealth > oldMaxHealth) // Tăng maxHealth
        {
            // Tùy chọn: tăng currentHealth theo tỷ lệ hoặc thêm vào
            float healthPercentage = (float)currentHealth / oldMaxHealth;
            currentHealth = Mathf.RoundToInt(newMaxHealth * healthPercentage);

            // Hoặc: currentHealth += (newMaxHealth - oldMaxHealth);
        }
        else if (newMaxHealth < oldMaxHealth) // Giảm maxHealth
        {
            if (currentHealth > newMaxHealth)
                currentHealth = newMaxHealth;
        }

        if (onHealthChanged != null)
            onHealthChanged();
    }
    #endregion

    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if(!isDead)
            Die();
    }

    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelligence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critRate) return critRate;
        else if (_statType == StatType.critDame) return critDame;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicResistance) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightingDamage;

        return null;
    }

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;
}

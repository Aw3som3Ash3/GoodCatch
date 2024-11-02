using System;
using System.Linq;
//using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Fish Monster", menuName = "Fish Monster/Fish Monster", order = 1)]
public class FishMonsterType : ScriptableObject
{
    [SerializeField]
    GameObject model;
    public GameObject Model { get { return model; } }

    public int fishId;

    [SerializeField]
    Sprite icon;
    public Sprite Icon { get { return icon; } }
    [SerializeField]
    Depth homeDepth;
    [SerializeField]
    Bait[] preferredBait;
    [SerializeField]
    Element[] elements;
    public Element[] Elements { get { return elements; } }

    [SerializeField]
    [Header("Agility")]
    int minAgility;
    [SerializeField]
    int maxAgility;
    [SerializeField]
    [Header("Attack")]
    int minAttack;
    [SerializeField]
    int maxAttack;
    [SerializeField]
    [Header("Special")]
    int minSpecial;
    [SerializeField]
    int maxSpecial;
    [SerializeField]
    [Header("Fortitude")]
    int minFortitude;
    [SerializeField]
    int maxFortitude;

    [SerializeField]
    [Header("SpecialFort")]
    int minSpecialFort;
    [SerializeField]
    int maxSpecialFort;

    [SerializeField]
    [Header("Accuracy")]
    int minAccuracy;
    [SerializeField]
    int maxAccuracy;


    [Header("Health")]
    [SerializeField]
    int healthPerLevel;
    public int HealthPerLevel { get { return healthPerLevel; } }
    [Header("Stamina")]
    [SerializeField]
    int staminaPerLevel;
    public int StaminaPerLevel { get { return staminaPerLevel; } }
    [SerializeField]
    Ability[] baseAbilities;

    [Header("Misc")]
    [SerializeField]
    int difficulty;





    public Ability[] BaseAbilities { get { return baseAbilities; } }

    public FishMonster GenerateMonster()
    {
        float value = Mathf.Clamp01(minAgility);
        int speed = UnityEngine.Random.Range(minAgility, maxAgility);
        int attack = UnityEngine.Random.Range(minAttack, maxAttack);
        int special = UnityEngine.Random.Range(minSpecial, maxSpecial);
        int fortitude = UnityEngine.Random.Range(minFortitude, maxFortitude);
        int specialFort = UnityEngine.Random.Range(minSpecialFort, maxSpecialFort);
        int accuracy = UnityEngine.Random.Range(minAccuracy, maxAccuracy);

        return new FishMonster(this, speed, attack, special, fortitude, specialFort, accuracy);
    }

}


[Serializable]
public class FishMonster
{

    FishMonsterType type;
    string name;
    public int agility { get; private set; }
    public int accuracy { get; private set; }
    public int attack { get; private set; }
    public int special { get; private set; }
    public int fortitude { get; private set; }
    public int specialFort { get; private set; }

    public float health { get; private set; }
    public float maxHealth { get; private set; }
    public float stamina { get; private set; }
    public float maxStamina { get; private set; }

    public Sprite Icon { get { return type.Icon; } }
    public GameObject Model { get { return type.Model; } }
    public int level { get; private set; } = 1;
    float xp;
    const int xpToLevelUp = 1000;
    Ability[] abilities;
    public Action ValueChanged;
    public Action HasFeinted;
    public bool isDead { get; set; } = false;

    public FishMonster(FishMonsterType monsterType, int agility, int attack, int special, int fortitude, int specialFort, int accuracy)
    {
        this.type = monsterType;
        name = monsterType.name;
        this.agility = agility;
        this.attack = attack;
        this.special = special;
        this.fortitude = fortitude;
        this.specialFort = specialFort;
        this.accuracy = accuracy;
        maxStamina = StaminaFormula();
        stamina = maxStamina;
        maxHealth = HealthFormula();
        health = maxHealth;
        abilities = monsterType.BaseAbilities;
    }
    public void RestoreAllHealth()
    {
        health = maxHealth;
    }
    public void ReplaceAbility(Ability newAbility, int index)
    {
        abilities[index] = newAbility;

    }
    public void ConsumeStamina(int amount)
    {
        stamina -= amount;
        ValueChanged?.Invoke();

    }
    public Ability GetAbility(int index)
    {
        if (index >= abilities.Length)
        {
            return null;
        }
        return abilities[index];
    }
    public void ChangeName(string newName)
    {
        name = newName;
    }

    public void AddXp(float xp)
    {

        this.xp += xp;
        if (this.xp > xpToLevelUp)
        {
            LevelUp();
        }
    }
    void LevelUp()
    {
        level++;
        xp = 0;
        maxHealth = HealthFormula();
        health = maxHealth;
        maxStamina = StaminaFormula();
        stamina = maxStamina;
    }
    int HealthFormula()
    {
        return level * type.HealthPerLevel;
    }
    int StaminaFormula()
    {
        return level * type.StaminaPerLevel;
    }
    public void RecoverStamina()
    {
        stamina += maxStamina / 4;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        ValueChanged?.Invoke();
    }
    public void TakeDamage(float damage, Element elementType, Ability.AbilityType abilityType)
    {
        if (damage <= 0)
        {
            Debug.Log("took no damage");
            return;
        }
        float defenseMod = 1 - (abilityType == Ability.AbilityType.attack ? fortitude : specialFort) * 0.01f;
        float damageTaken = damage * DamageModifier(elementType) * defenseMod;
        health -= damageTaken;
        Debug.Log("took " + damageTaken + " damage \n current health: " + health);
        if (health <= 0)
        {
            Feint();
        }
        ValueChanged?.Invoke();
    }
    public void Restore(int health = 0, int stamina = 0)
    {
        this.health += health;
        this.stamina += stamina;
        ValueChanged?.Invoke();
    }
    float DamageModifier(Element elementType)
    {
        if (elementType == null)
        {
            return 1;
        }
        return type.Elements.OrderByDescending(e => e.CompareStrength(elementType)).First().DamageModifier(elementType);
    }
    void Feint()
    {
        isDead = true;
        HasFeinted?.Invoke();
        Debug.Log("Should Feint or die");
    }

    public override string ToString()
    {
        return name;
    }

}

[Flags]
public enum Depth
{
    shallow = 1 << 0,
    middle = 1 << 1,
    abyss = 1 << 2

}

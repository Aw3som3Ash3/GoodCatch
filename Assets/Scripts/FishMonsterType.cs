using System;
using System.Linq;
//using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Fish Monster", menuName = "Fish Monster/Fish Monster", order = 1)]
public class FishMonsterType : ScriptableObject
{
    
    [Serializable]
    struct Attribute
    {
        [SerializeField]
        int min, max;
        public int Min { get { return min; } }
        public int Max { get { return max; } }
        [SerializeField]
        TalentScale minTalent, maxTalent;

        public TalentScale MinTalent { get { return minTalent; } }
        public TalentScale MaxTalent { get { return maxTalent; } }

    }


    [Header("Description")]
    [SerializeField]
    string description;
    public String Description { get { return description; } }

    [Header("variables")]
    [SerializeField]
    GameObject model;
    public GameObject Model { get { return model; } }

    public int fishId;

    [SerializeField]
    Texture2D icon;
    public Texture2D Icon { get { return icon; } }
    [SerializeField]
    Depth homeDepth;
    [SerializeField]
    Bait[] preferredBait;
    [SerializeField]
    Element[] elements;
    public Element[] Elements { get { return elements; } }



    [SerializeField]
    [Header("Agility")]
    Attribute agility;

    [SerializeField]
    [Header("Attack")]
    Attribute attack;

    [SerializeField]
    [Header("Special")]
    Attribute special;

    [SerializeField]
    [Header("Fortitude")]
    Attribute fortitude;

    [SerializeField]
    [Header("SpecialFort")]
    Attribute SpecialFortitude;

    [SerializeField]
    [Header("Accuracy")]
    Attribute accuracy;


    [Header("Health")]
    [SerializeField]
    int baseHealth;
    public int BaseHealth { get { return baseHealth; } }
    [Header("Stamina")]
    [SerializeField]
    int baseStamina;
    public int BaseStamina { get { return baseStamina; } }
    [SerializeField]
    Ability[] baseAbilities;

    [Header("Misc")]
    [SerializeField]
    int difficulty;





    public Ability[] BaseAbilities { get { return baseAbilities; } }

    int RandomAttributeValue(Attribute attribute)
    {
        return UnityEngine.Random.Range(attribute.Min, attribute.Max);
    }
    TalentScale CalculateTalent(Attribute attribute)
    {
        TalentScale talent;
        int x = UnityEngine.Random.Range((int)attribute.MinTalent*10, (int)attribute.MaxTalent * 10) + UnityEngine.Random.Range((int)attribute.MinTalent * 10, (int)attribute.MaxTalent * 10);
        if (x >= 95)
        {
            talent = TalentScale.S;
        }
        else if (x > 80)
        {
            talent = TalentScale.A;
        }
        else if (x > 60)
        {
            talent = TalentScale.B;
        }
        else if (x > 40)
        {
            talent = TalentScale.C;
        }
        else if (x > 20)
        {
            talent = TalentScale.D;
        }
        else
        {
            talent = TalentScale.F;
        }

        return talent;
    }
    public FishMonster GenerateMonster()
    {

        int speed = RandomAttributeValue(agility);
        TalentScale agilityTalent= CalculateTalent(agility);

        int attack = RandomAttributeValue(this.attack);
        TalentScale attackTalent = CalculateTalent(this.attack);

        int special = RandomAttributeValue(this.special);
        TalentScale specialTalent = CalculateTalent(this.special);

        int fortitude = RandomAttributeValue(this.fortitude);
        TalentScale fortitudeTalent = CalculateTalent(this.fortitude);

        int specialFort = RandomAttributeValue(this.SpecialFortitude);
        TalentScale specialFortTalent = CalculateTalent(this.SpecialFortitude);

        int accuracy = RandomAttributeValue(this.accuracy);
        TalentScale accuracyTalent = CalculateTalent(this.accuracy);

        return new FishMonster(this, speed,agilityTalent ,attack,attackTalent ,special,specialTalent, fortitude, fortitudeTalent, specialFort, specialFortTalent,accuracy, accuracyTalent);
    }

}
public enum TalentScale
{
    F,
    D,
    C,
    B,
    A,
    S

}
[Serializable]
public class FishMonster
{
    

    FishMonsterType type;
    public string name { get; private set; }
    public string description { get { return type.Description; } }

    [Serializable]
    public struct Attribute
    {
        public int value;
        public TalentScale talent;
    }

    Attribute agility;
    public Attribute Agility { get { return agility; } }

    Attribute accuracy;
    public Attribute Accuracy { get;}

    Attribute attack;
    public Attribute Attack { get { return attack; } }

    Attribute special;
    public Attribute Special { get { return special; } }

    Attribute fortitude;
    public Attribute Fortitude { get { return fortitude; } }

    Attribute specialFort;
    public Attribute SpecialFort{ get { return specialFort; } }

    public float health { get; private set; }
    public float maxHealth { get; private set; }
    public float stamina { get; private set; }
    public float maxStamina { get; private set; }

    public Texture2D Icon { get { return type.Icon; } }
    public GameObject Model { get { return type.Model; } }
    public int level { get; private set; } = 1;
    public float xp { get; private set; }
    public const int xpToLevelUp = 1000;
    Ability[] abilities;
    public Action ValueChanged;
    public Action HasFeinted;
    public bool isDead { get; set; } = false;

    public FishMonster(FishMonsterType monsterType, int agility,TalentScale agilityTalent, int attack, TalentScale attackTalent, int special, TalentScale specialTalent, int fortitude, TalentScale fortitudeTalent, int specialFort, TalentScale specialFortTalent, int accuracy,TalentScale accuracyTalent)
    {

        this.type = monsterType;
        name = monsterType.name;
        
        this.agility.value = agility;
        this.agility.talent = agilityTalent;
        this.attack.value = attack;
        this.attack.talent= attackTalent;
        this.special.value = special;
        this.special.talent= specialTalent;
        this.fortitude.value = fortitude;
        this.fortitude.talent = fortitudeTalent;
        this.specialFort.value = specialFort;
        this.specialFort.talent = specialFortTalent;
        this.accuracy.value = accuracy;
        this.accuracy.talent = accuracyTalent;
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
        LevelAttribute(agility);
        LevelAttribute(attack);
        LevelAttribute(fortitude);
        LevelAttribute(special);
        LevelAttribute(specialFort);
        LevelAttribute(accuracy);


    }
    void LevelAttribute(Attribute attribute)
    {
        attribute.value += (int)attribute.talent+1;
    }
    int HealthFormula()
    {
        return type.BaseHealth + (level - 1) * type.BaseHealth/10;
    }
    int StaminaFormula()
    {
        return type.BaseStamina + (level - 1) * type.BaseStamina/10;
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
        float defenseMod = 1 - (abilityType == Ability.AbilityType.attack ? fortitude.value : specialFort.value) * 0.01f;
        float damageTaken = damage * DamageModifier(elementType) * defenseMod;
        health -= damageTaken;
        Debug.Log("took " + damageTaken + " damage \n current health: " + health);
        if (health <= 0)
        {
            Feint();
        }
        ValueChanged?.Invoke();
    }
    public void Restore(float health = 0, float stamina = 0)
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

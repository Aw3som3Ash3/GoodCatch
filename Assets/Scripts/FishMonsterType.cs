using System;
using System.Linq;
using UnityEditor;
//using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Fish Monster", menuName = "Fish Monster/Fish Monster", order = 1)]
public class FishMonsterType : ScriptableObject
{
    
    [Serializable]
    public struct Attribute
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
    public Attribute Agility { get { return agility; } }

    [SerializeField]
    [Header("Attack")]
    Attribute attack;
    public Attribute Attack { get { return attack; } }

    [SerializeField]
    [Header("Special")]
    Attribute special;
    public Attribute Special { get { return special; } }

    [SerializeField]
    [Header("Fortitude")]
    Attribute fortitude;
    public Attribute Fortitude { get {  return fortitude; } }


    [SerializeField]
    [Header("SpecialFort")]
    Attribute specialFortitude;
    public Attribute SpecialFortitude { get { return specialFortitude; } }

    [SerializeField]
    [Header("Accuracy")]
    Attribute accuracy;
    public Attribute Accuracy { get { return accuracy; } }


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
    [SerializeField]
    AnimationClip attackAnimation,idleAnimation;
    public AnimationClip AttackAnimation { get { return attackAnimation; } }
    public AnimationClip IdleAnimation { get { return idleAnimation; } }





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

        int specialFort = RandomAttributeValue(this.specialFortitude);
        TalentScale specialFortTalent = CalculateTalent(this.specialFortitude);

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
    FishMonsterType Type 
    { 
        get 
        { 
            if (type == null) 
            {
                type = GameManager.Instance.Database.fishMonsters[id];
            } 
            return type;
        } set 
        {
            type = value; 
        } 
    }
    [SerializeField]
    int id;
    [SerializeField]
    string name;
    public string Name { get { return name; } }
    public string description { get { return Type.Description; } }

    [Serializable]
    public struct Attribute
    {
        public int value;
        public TalentScale talent;
    }
    [SerializeField]
    Attribute agility;
    public Attribute Agility { get { return agility; } }
    [SerializeField]
    Attribute accuracy;
    public Attribute Accuracy { get;}
    [SerializeField]
    Attribute attack;
    public Attribute Attack { get { return attack; } }
    [SerializeField]
    Attribute special;
    public Attribute Special { get { return special; } }
    [SerializeField]
    Attribute fortitude;
    public Attribute Fortitude { get { return fortitude; } }
    [SerializeField]
    Attribute specialFort;
    public Attribute SpecialFort{ get { return specialFort; } }
    [SerializeField]
    float health;
    public float Health { get { return health; } }
    [SerializeField]
    float maxHealth;
    public float MaxHealth { get { return maxHealth; } }
    [SerializeField]
    float stamina;
    public float Stamina { get { return stamina; } }
    [SerializeField]
    float maxStamina;
    public float MaxStamina { get { return maxStamina; } }
    public float Dodge { get { return agility.value / 2; } }
    public Texture2D Icon { get { return Type.Icon; } }
    public GameObject Model { get { return Type.Model; } }

    int level = 1;
    public int Level { get { return level; } }
    float xp;
    public float Xp { get { return xp; } }
    public const int xpToLevelUp = 1000;
    Ability[] abilities;
    public Action ValueChanged;
    public Action HasFeinted;
    public bool isDead { get { return health <= 0; }  }
    public AnimationClip AttackAnimation { get { return type.AttackAnimation; } }
    public AnimationClip IdleAnimation { get { return type.IdleAnimation; } }

    public FishMonster(FishMonsterType monsterType, int agility,TalentScale agilityTalent, int attack, TalentScale attackTalent, int special, TalentScale specialTalent, int fortitude, TalentScale fortitudeTalent, int specialFort, TalentScale specialFortTalent, int accuracy,TalentScale accuracyTalent)
    {

        this.Type = monsterType;
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
        stamina = MaxStamina;
        maxHealth = HealthFormula();
        health = MaxHealth;
        abilities = monsterType.BaseAbilities;
    }
    public void RestoreAllHealth()
    {
        health = MaxHealth;
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
        if (this.Xp > xpToLevelUp)
        {
            LevelUp();
        }
    }
    void LevelUp()
    {
        level++;
        xp = 0;
        maxHealth = HealthFormula();
        health = MaxHealth;
        maxStamina = StaminaFormula();
        stamina = MaxStamina;
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
        return Type.BaseHealth + (Level - 1) * Type.BaseHealth/10;
    }
    int StaminaFormula()
    {
        return Type.BaseStamina + (Level - 1) * Type.BaseStamina/10;
    }
    public void RecoverStamina()
    {
        stamina += MaxStamina / 4;
        stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
        ValueChanged?.Invoke();
    }
    public void TakeDamage(float damage, Element elementType, Ability.AbilityType abilityType)
    {
        if (damage <= 0)
        {
            Debug.Log("took no damage");
            return;
        }
        //float defenseMod = 1 - (abilityType == Ability.AbilityType.attack ? fortitude.value : specialFort.value) * 0.01f;
        float defenseMod =MathF.Pow(MathF.E,-0.015f* (abilityType == Ability.AbilityType.attack ? fortitude.value : specialFort.value));
        float damageTaken = damage * DamageModifier(elementType) * defenseMod;
        health -= damageTaken;
        Debug.Log("took " + damageTaken + " damage \n current health: " + Health);
        if (Health <= 0)
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
        return Type.Elements.OrderByDescending(e => e.CompareStrength(elementType)).First().DamageModifier(elementType);
    }
    void Feint()
    {
        //isDead = true;
        HasFeinted?.Invoke();
        Debug.Log("Should Feint or die");
    }

    public override string ToString()
    {
        return Name;

       
    }

}

[Flags]
public enum Depth
{
    shallow = 1 << 0,
    middle = 1 << 1,
    abyss = 1 << 2

}

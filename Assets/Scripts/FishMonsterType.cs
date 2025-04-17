using System;
using System.Linq;
using Unity.Mathematics;
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
    [Multiline]
    string description;
    public String Description { get { return description; } }

    [Header("variables")]
    [SerializeField]
    GameObject model;
    public GameObject Model { get { return model; } }

    public int fishId;

    [SerializeField]
    Sprite icon;
    public Sprite Icon { get { return icon; } }

    [SerializeField]
    Sprite miniSprite;
    public Sprite MiniSprite { get { return miniSprite; } }
    [SerializeField]
    Depth homeDepth;
    public Depth HomeDepth { get { return homeDepth; } }
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

    [Serializable]
    public struct AbilityOptions
    {
        [SerializeField]
        public Ability[] potentialAbilities;

        public Ability GetAbility()
        {
            if (potentialAbilities ==null|| potentialAbilities.Length == 0) 
            {
                Debug.LogError("No Ability");
                return null;
            }
            int index = potentialAbilities.Length > 1? UnityEngine.Random.Range(0, potentialAbilities.Length):0;
            
            return potentialAbilities[index];
        }
    }


    [SerializeField]
    AbilityOptions[] baseAbilities;
    public AbilityOptions[] BaseAbilities { get => baseAbilities; }
    [Header("Misc")]
    [SerializeField]
    int difficulty;
    [SerializeField]
    AnimationClip attackAnimation,idleAnimation,buffAnimation;
    public AnimationClip AttackAnimation { get { return attackAnimation; } }
    public AnimationClip IdleAnimation { get { return idleAnimation; } }
    public AnimationClip BuffAnimation { get { return buffAnimation; } }





    //public Ability[] BaseAbilities { get { return baseAbilities; } }

    int RandomAttributeValue(Attribute attribute)
    {
        return UnityEngine.Random.Range(attribute.Min, attribute.Max);
    }
    TalentScale CalculateTalent(Attribute attribute)
    {
        TalentScale talent;

        int max=0;
        int min=0;

        switch (attribute.MaxTalent)
        {
            case TalentScale.F:
                max = 20;
                break;
            case TalentScale.D:
                max = 40;
                break;
            case TalentScale.C:
                max = 60;
                break;
            case TalentScale.B:
                max = 80;
                break;
            case TalentScale.A:
                max = 95;
                break;
            case TalentScale.S:
                max = 100;
                break;
        }
        switch (attribute.MinTalent)
        {
            case TalentScale.F:
                min = 0;
                break;
            case TalentScale.D:
                min = 21;
                break;
            case TalentScale.C:
                min = 41;
                break;
            case TalentScale.B:
                min = 61;
                break;
            case TalentScale.A:
                min = 81;
                break;
            case TalentScale.S:
                min = 96;
                break;
        }

        int x = UnityEngine.Random.Range(min, max+1);


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

    public FishMonster GenerateMonster(int startingLevel = 1)
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

        return new FishMonster(this, speed,agilityTalent ,attack,attackTalent ,special,specialTalent, fortitude, fortitudeTalent, specialFort, specialFortTalent,accuracy, accuracyTalent,GenerateAbilities(),startingLevel);
    }

    Ability[] GenerateAbilities()
    {
        Ability[] abilities = new Ability[baseAbilities.Length];
       
        for (int i = 0; i < baseAbilities.Length; i++)
        {
            abilities[i] = baseAbilities[i].GetAbility();
            if (abilities[i] == null)
            {
                Debug.LogError($"NO ABILITIES ON {name} SLOT {i}");
            }
        }
        return abilities;
    }



}
public enum TalentScale
{
    F=1,
    D=2,
    C=3,
    B=5,
    A=8,
    S=13

}
[Serializable]
public class FishMonster
{

    FishMonsterType type;
    public FishMonsterType Type 
    { 
        get 
        { 
            if (type == null) 
            {
                type = GameManager.Instance.Database.fishMonsters[id];
            } 
            return type;
        } private set 
        {
            type = value; 
        } 
    }
    [SerializeField]
    int id;
    public int ID { get { return id; } }
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
    public Attribute Accuracy { get { return accuracy; } }
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
    public float Dodge { get { return agility.value / 3; } }
    public Sprite Icon { get { return Type.Icon; } }
    public Sprite MiniSprite { get { return Type.MiniSprite; } }
    public GameObject Model { get { return Type.Model; } }

    [SerializeField]
    int level = 1;
    public int Level { get { return level; } }
    [SerializeField]
    float xp;
    public float Xp { get { return xp; } }
    public const int xpToLevelUp = 1000;
    //[SerializeField]
    Ability[] abilities;
    Ability[] Abilities 
    { 
        get 
        {
            if (abilities == null)
            {
                abilities = new Ability[abilityIds.Length];
                for (int i = 0;i<abilityIds.Length;i++)
                {
                    abilities[i] = Ability.getAbilityById[abilityIds[i]];
                }
            }
            return abilities;
        } 
    }
    [SerializeField]
    string[] abilityIds;
    public Action ValueChanged;
    public Action HasFeinted;
    public bool isDead { get { return health <= 0; }  }
    public AnimationClip AttackAnimation { get { return type.AttackAnimation; } }
    public AnimationClip IdleAnimation { get { return type.IdleAnimation; } }
    public AnimationClip BuffAnimation { get { return type.BuffAnimation; } }

    public FishMonster(FishMonsterType monsterType, int agility,TalentScale agilityTalent, int attack, TalentScale attackTalent, int special, TalentScale specialTalent, int fortitude, TalentScale fortitudeTalent, int specialFort, TalentScale specialFortTalent, int accuracy,TalentScale accuracyTalent, Ability[] abilities,int startingLevel)
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
        
        this.abilities = abilities;
        abilityIds = new string[abilities.Length];
        for(int i = 1; i < startingLevel; i++)
        {
            LevelUp();
        }
        for(int i = 0; i < abilityIds.Length; i++)
        {
            Debug.Log("id :" + abilities[i]);
            abilityIds[i] = abilities[i].AbilityID;
        }
        id = type.fishId;
    }
    public void RestoreAllHealth()
    {
        health = MaxHealth;
    }
    public void ReplaceAbility(Ability newAbility, int index)
    {
        Abilities[index] = newAbility;
        
    }
    public void ConsumeStamina(int amount)
    {
        stamina -= amount;
        ValueChanged?.Invoke();

    }
    public Ability GetAbility(int index)
    {
        if (index >= Abilities.Length)
        {
            return null;
        }
        return Abilities[index];
    }
    public void ChangeName(string newName)
    {
        name = newName;
    }

    public void AddXp(float xp)
    {

        this.xp += xp;
        if (this.Xp >= xpToLevelUp)
        {
            LevelUp();
        }
    }
    void LevelUp()
    {
        level++;
        xp %=xpToLevelUp;
        maxHealth = HealthFormula();
        health = MaxHealth;
        maxStamina = StaminaFormula();
        stamina = MaxStamina;
        LevelAttribute(ref agility);
        LevelAttribute(ref attack);
        LevelAttribute(ref fortitude);
        LevelAttribute(ref special);
        LevelAttribute(ref specialFort);
        LevelAttribute(ref accuracy);


    }
    void LevelAttribute(ref Attribute attribute)
    {
        Debug.Log("should level attribute to" +attribute.talent);
        attribute.value += (int)attribute.talent;
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
    public void RestoreStaminaFull()
    {
        stamina = maxStamina;
    }
    public float TakeDamage(float damage, Element elementType, Ability.AbilityType abilityType,out Element.Effectiveness effectiveness)
    {
        if (damage <= 0)
        {
            Debug.Log("took no damage");
            effectiveness = Element.Effectiveness.none;
            return 0;
        }
        //float defenseMod = 1 - (abilityType == Ability.AbilityType.attack ? fortitude.value : specialFort.value) * 0.01f;
        float defenseMod =MathF.Pow(MathF.E,-0.015f* (abilityType == Ability.AbilityType.attack ? fortitude.value : specialFort.value));
        float damageTaken = damage * DamageModifier(elementType) * defenseMod;
        effectiveness = GetEffectiveness(elementType);
        //health -= damageTaken;
        Debug.Log("took " + damageTaken + " damage \n current health: " + Health);
        //ValueChanged?.Invoke();
        return damageTaken;
    }

    public void UpdateHealth(float health)
    {
        this.health = health;
    }

  
    public void Restore(float health = 0, float stamina = 0)
    {
        this.health =Mathf.Clamp(this.health+health,0,MaxHealth);
        this.stamina = Mathf.Clamp(this.stamina+stamina, 0, maxStamina);
        ValueChanged?.Invoke();
    }
    float DamageModifier(Element elementType)
    {
        if (elementType == null||type.Elements.Length<=0)
        {
            return 1;
        }
        return Type.Elements.OrderByDescending(e => e.CompareStrength(elementType)).First().DamageModifier(elementType);
    }
    Element.Effectiveness GetEffectiveness(Element elementType)
    {
        if (elementType == null)
        {
            return Element.Effectiveness.none;
        }
        return Type.Elements.OrderByDescending(e => e.CompareStrength(elementType)).First().GetEffectiveness(elementType);
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

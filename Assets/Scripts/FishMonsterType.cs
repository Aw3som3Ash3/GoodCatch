using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
//using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Fish Monster", menuName = "Fish Monster/Fish Monster", order = 1)]
public class FishMonsterType : ScriptableObject
{
    [SerializeField]
    GameObject model;
    [SerializeField]
    Image icon;
    [SerializeField]
    Depth homeDepth;
    [SerializeField]
    Element[] elements;
    public Element[] Elements { get { return elements; } }
    
    [SerializeField]
    [Header("Speed")]
    int minSpeed;
    [SerializeField]
    int maxSpeed;
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
    Ability[] baseAbilities;
    public Ability[] BaseAbilities { get { return baseAbilities; } }

    public FishMonster GenerateMonster()
    {
        float value= Mathf.Clamp01(minSpeed);
        int speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        int attack = UnityEngine.Random.Range(minAttack, maxAttack);
        int special = UnityEngine.Random.Range(minSpecial, maxSpecial);
        int fortitude = UnityEngine.Random.Range(minFortitude, maxFortitude);
        int specialFort = UnityEngine.Random.Range(minSpecialFort, maxSpecialFort);

        return new FishMonster(this,speed, attack, special, fortitude, specialFort);
    }

}


[Serializable]
public class FishMonster
{
    
    FishMonsterType type;
    string name;
    int speed,attack,special,fortitude,specialFort;
    float health, maxHealth;
    int level;
    float xp;
    const int xpToLevelUp=1000;
    Ability[] abilities;
    Dictionary<Ability, int> abilityUsage = new Dictionary<Ability, int>();
    public FishMonster(FishMonsterType monsterType, int speed,int attack,int special,int fortitude, int specialFort)
    {
        this.type = monsterType;
        name=monsterType.name;
        this.speed = speed;
        this.attack = attack;
        this.special = special;
        this.fortitude = fortitude;
        this.specialFort = specialFort;
        maxHealth = HealthFormula();
        health = maxHealth;
        abilities = monsterType.BaseAbilities;
        foreach ( Ability ability in abilities)
        {
            abilityUsage[ability] = ability.MaxUsages;
        }

        
    }
    public void ReplaceAbility(Ability newAbility, int index)
    {
        abilityUsage.Remove(abilities[index]);
        abilityUsage[newAbility] = newAbility.MaxUsages;
        abilities[index]=newAbility;

    }

    public void UseAbility(int index)
    {
        if (abilityUsage[abilities[index]] > 0)
        {
            abilities[index].UseAbility();
            abilityUsage[abilities[index]]--;
        }
       
    }
    public void ChangeName(string newName)
    {
        name=newName;
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
        maxHealth=HealthFormula();
        health = maxHealth;
    }
    int HealthFormula()
    {
        return level * 100;
    }

    public void TakeDamage(float damage, Element elementType)
    {
        if (damage<=0)
        {
            Debug.Log("took no damage");
            return;
        }
        health -= damage*DamageModifier(elementType);
        if (health < 0)
        {
            Feint();
        }
    }

    float DamageModifier(Element elementType)
    {
        return type.Elements.OrderByDescending(e => e.CompareStrength(elementType)).First().DamageModifier(elementType);
    }
    void Feint()
    {
        Debug.Log("Should Feint or die");
    }
}

[Flags]
public enum Depth
{
    shallow=1<<0,
    middile=1<<1,
    abyss=1<<2

}

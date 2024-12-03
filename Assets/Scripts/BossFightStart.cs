using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossFightStart : MonoBehaviour,ISaveable
{
    [SerializeField] FishMonsterType fishMonster1;
    [SerializeField] FishMonsterType fishMonster2;
    [SerializeField] FishMonsterType fishMonster3;


    void OnTriggerEnter(Collider other)
    {
        List<FishMonster> fishMonsters = new List<FishMonster>();

        fishMonsters.Add(fishMonster.GenerateMonster());


        if (other.gameObject.tag == "Player")
        {
            GameManager.Instance.LoadCombatScene(fishMonsters);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossFightStart : MonoBehaviour
{
    [SerializeField] FishMonsterType fishMonster1;
    [SerializeField] FishMonsterType fishMonster2;
    [SerializeField] FishMonsterType fishMonster3;


    void OnTriggerEnter(Collider other)
    {
        List<FishMonster> fishMonsters = new List<FishMonster>();

        fishMonsters.Add(fishMonster1.GenerateMonster(10));
        fishMonsters.Add(fishMonster2.GenerateMonster(10));
        fishMonsters.Add(fishMonster3.GenerateMonster(10));


        if (other.gameObject.tag == "Player")
        {
            this.gameObject.SetActive(false);
            GameManager.Instance.LoadCombatScene(fishMonsters);
            
            Debug.Log("Boss destroyed.");
        }
    }


    /*void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.gameObject.SetActive(false);
        }
    }*/
}

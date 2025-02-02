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

        fishMonsters.Add(fishMonster1.GenerateMonster());
        fishMonsters.Add(fishMonster2.GenerateMonster());
        fishMonsters.Add(fishMonster3.GenerateMonster());


        if (other.gameObject.tag == "Player")
        {
            GameManager.Instance.LoadCombatScene(fishMonsters);
            this.gameObject.SetActive(false);
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

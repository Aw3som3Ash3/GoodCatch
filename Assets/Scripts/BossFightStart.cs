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
        if(fishMonster1!=null)
            fishMonsters.Add(fishMonster1.GenerateMonster());
        if (fishMonster2 != null)
            fishMonsters.Add(fishMonster2.GenerateMonster());
        if(fishMonster3!=null)
            fishMonsters.Add(fishMonster3.GenerateMonster());


        if (other.gameObject.tag == "Player")
        {
            this.gameObject.SetActive(false);
            GameManager.Instance.LoadBossCombatScene(fishMonsters,ID);
            Debug.Log("Boss destroyed.");
        }
    }


//    void OnTriggerExit(Collider other)
//    {
//        if (other.gameObject.tag == "Player")
//        {
//            this.gameObject.SetActive(false);
//        }
//    }
}

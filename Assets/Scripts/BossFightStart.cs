using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossFightStart : TransformSaver
{
    [SerializeField] FishMonsterType fishMonster1;
    [SerializeField] FishMonsterType fishMonster2;
    [SerializeField] FishMonsterType fishMonster3;
    [SerializeField]
    int startLevel1, startLevel2, startLevel3;
    [SerializeField]
    bool isFinalBoss;

    private void Awake()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        List<FishMonster> fishMonsters = new List<FishMonster>();
        if(fishMonster1!=null)
            fishMonsters.Add(fishMonster1.GenerateMonster(startLevel1));
        if (fishMonster2 != null)
            fishMonsters.Add(fishMonster2.GenerateMonster(startLevel2));
        if(fishMonster3!=null)
            fishMonsters.Add(fishMonster3.GenerateMonster(startLevel3));


        if (other.gameObject.tag == "Player")
        {
            this.gameObject.SetActive(false);
            if (isFinalBoss)
            {
                GameManager.Instance.LoadBossCombatScene(fishMonsters, ID);
            }
            else
            {
                GameManager.Instance.LoadEndBossCombatScene(fishMonsters, ID);
            }
           
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

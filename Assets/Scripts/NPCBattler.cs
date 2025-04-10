using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBattler : MonoBehaviour
{
    [SerializeField]
    DialogueEvent startBattleEvent;
    [SerializeField]
    List<FishToFight> fishToFight;

    [Serializable]
    public struct FishToFight
    {
        [SerializeField]
        public FishMonsterType fishMonsters;
        [SerializeField]
        public int level;
    }


    void StartFight()
    {
        List<FishMonster> monsters = new List<FishMonster>();
        foreach (var fishType in fishToFight)
        {
            var fish = fishType.fishMonsters.GenerateMonster(fishType.level);
            monsters.Add(fish);
        }
        GameManager.Instance.LoadCombatScene(monsters);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (startBattleEvent != null) 
        {
            startBattleEvent.Event += StartFight;
        }
    }
    private void OnDestroy()
    {
        startBattleEvent.Event -= StartFight;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

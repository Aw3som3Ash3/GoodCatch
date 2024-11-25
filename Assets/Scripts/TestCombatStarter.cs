using System.Collections.Generic;
using UnityEngine;

public class TestCombatStarter : MonoBehaviour
{

    [SerializeField]
    FishMonsterType fishType;
    List<FishMonster> fishMonsters = new List<FishMonster>();
    // Start is called before the first frame update
    void Start()
    {
        fishMonsters.Add(fishType.GenerateMonster());
        //fishMonsters.Add(fishType.GenerateMonster());
        //fishMonsters.Add(fishType.GenerateMonster());
        Invoke("StartFight", 5);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartFight()
    {

        GameManager.Instance.LoadCombatScene(fishMonsters, true);
    }
}

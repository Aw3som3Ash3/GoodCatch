using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestCombatStarter : MonoBehaviour
{
    [SerializeField]
    InputAction action;
    [SerializeField]
    FishMonsterType fishType;
    [SerializeField] int numOfFish;
    List<FishMonster> fishMonsters = new List<FishMonster>();
    // Start is called before the first frame update
    void Start()
    {
        action.performed += (x) => StartFight();
        for (int i = 0; i < numOfFish; i++)
        {
            fishMonsters.Add(fishType.GenerateMonster());
        }
        action.Enable();
        //fishMonsters.Add(fishType.GenerateMonster());
        //fishMonsters.Add(fishType.GenerateMonster());
        //Invoke("StartFight", 5);
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

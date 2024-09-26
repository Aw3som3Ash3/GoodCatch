using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    Fishventory playerFishventory;

    List<FishMonster> fishesToFight;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void LoadCombatScene(List<FishMonster> enemyFishes)
    {
        SceneManager.LoadScene("BattleScene");
        SceneManager.sceneLoaded += SetUpCombat;
    }

    private void SetUpCombat(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "BattleScene")
        {
            GameObject.FindObjectOfType<CombatManager>().NewCombat(playerFishventory.Fishies.ToList(), fishesToFight);
        }
        //throw new NotImplementedException();
    }

    
}

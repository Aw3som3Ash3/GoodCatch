using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    enum Turn 
    {
        player,
        enemey
    }
    Turn currentTurn;
    int roundNmber;
    [SerializeField]
    [ReadOnly]
    List<FishMonster> playerShallows, enemyShallows, playerMiddle, enemyMiddle, playerAbyss, enemyAbyss;

    [SerializeField]
    Transform playerShallowsLocation, enemyShallowsLocation, playerMiddleLocation, enemyMiddleLocation, playerAbyssLocation, enemyAbyssLocation;

}

using UnityEngine;

public class CombatAI : MonoBehaviour
{
    CombatManager combatManager;

    EnemyTurn currentTurn;

    public void SetCombatManager(CombatManager combatManager)
    {
        this.combatManager = combatManager;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartTurn(EnemyTurn turn)
    {
        currentTurn = turn;

        Invoke("Logic", 2);
    }

    public void Logic()
    {
        combatManager.CompletedAllActions -= Logic;
        if (currentTurn.actionsLeft > 0)
        {
            CombatManager.Turn weakestTarget = combatManager.depths[0].TargetFirst(CombatManager.Team.player);
            int depthIndex = 0;
            for (int i = 1; i < combatManager.depths.Length; i++)
            {
                if (combatManager.depths[i].TargetFirst(CombatManager.Team.player)?.Health < weakestTarget?.Health || weakestTarget == null)
                {
                    weakestTarget = combatManager.depths[i].TargetFirst(CombatManager.Team.player);
                    depthIndex = i;
                }
            }
            currentTurn.UseAbility(0, depthIndex);
            combatManager.CompletedAllActions += Logic;
        }
        else
        {
            currentTurn.EndTurn();
        }


    }
}

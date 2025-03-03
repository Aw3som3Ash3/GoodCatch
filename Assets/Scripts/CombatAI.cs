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
            
            int abilityIndex = 0;
            int tryCount=0;
            do
            {
                if (tryCount >=5)
                {
                    break;
                }
                abilityIndex = Random.Range(0, 3);
                tryCount++;
               


            } while (!currentTurn.AbilityUsable(abilityIndex));


            if(currentTurn.fish.GetAbility(abilityIndex).TargetedTeam == Ability.TargetTeam.self)
            {
                depthIndex=currentTurn.depthIndex;
            }
            else
            {
                for (int i = 1; i < combatManager.depths.Length; i++)
                {
                    if (currentTurn.fish.GetAbility(abilityIndex).TargetedTeam == Ability.TargetTeam.enemy && combatManager.depths[i].TargetFirst(CombatManager.Team.player)?.Health < weakestTarget?.Health || weakestTarget == null)
                    {
                        weakestTarget = combatManager.depths[i].TargetFirst(CombatManager.Team.player);
                        depthIndex = i;
                    }
                    else if (currentTurn.fish.GetAbility(abilityIndex).TargetedTeam == Ability.TargetTeam.friendly && (combatManager.depths[i].TargetFirst(CombatManager.Team.enemy)?.Health < weakestTarget?.Health || weakestTarget == null))
                    {
                        weakestTarget = combatManager.depths[i].TargetFirst(CombatManager.Team.enemy);
                        depthIndex = i;

                    }
                }
            }
            

            if (currentTurn.AbilityUsable(abilityIndex))
            {
                currentTurn.UseAbilityDirect(abilityIndex, depthIndex);
                combatManager.CompletedAllActions += Logic;
            }
            else
            {
                currentTurn.EndTurn();
            }
            

            
        }
        else
        {
            currentTurn.EndTurn();
        }


    }
}

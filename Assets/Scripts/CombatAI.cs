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
        EnemyTurn.TurnEnded += OnTurnEnded;
        //Logic();
        Invoke("Logic", 2);
    }
    void OnTurnEnded()
    {
        CancelInvoke();
        EnemyTurn.TurnEnded -= OnTurnEnded;
    }
    private void OnDestroy()
    {
        EnemyTurn.TurnEnded -= OnTurnEnded;
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
                    Invoke("EndTurn", 2);
                  
                    return;
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
                    if (!currentTurn.DepthTargetable(abilityIndex, combatManager.depths[i].depth))
                    {
                        continue;
                    }
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
            

            if (currentTurn.AbilityUsable(abilityIndex)&& weakestTarget!=null && currentTurn.DepthTargetable(abilityIndex, combatManager.depths[weakestTarget.depthIndex].depth))
            {
                currentTurn.UseAbilityDirect(abilityIndex, depthIndex);
                combatManager.CompletedAllActions += Logic;
            }else if (!currentTurn.fish.Type.HomeDepth.HasFlag(currentTurn.currentDepth.depth))
            {
                combatManager.CompletedAllActions += Logic;
                int targetIndex=0;
                switch (currentTurn.fish.Type.HomeDepth)
                {
                    case Depth.shallow:
                        targetIndex = combatManager.depthIndex[combatManager.depth[Depth.shallow]];
                        break;
                    case Depth.middle:
                        targetIndex = combatManager.depthIndex[combatManager.depth[Depth.middle]];
                        break;
                    case Depth.abyss:
                        targetIndex = combatManager.depthIndex[combatManager.depth[Depth.abyss]];
                        break;
                }
                
                currentTurn.Move(targetIndex); 

            }
            else
            {
                Invoke("EndTurn", 2);
            }



        }
        else
        {
            Invoke("EndTurn", 2);
        }


    }



    void EndTurn()
    {
        currentTurn.EndTurn();
    }
}

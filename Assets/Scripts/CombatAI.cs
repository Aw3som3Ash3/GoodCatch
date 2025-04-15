using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAI : MonoBehaviour
{
    CombatManager combatManager;

    EnemyTurn currentTurn;
    bool actionPending = false;

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
        actionPending = false;
        StartCoroutine(Logic());
        
        //Logic();
        //Invoke("Logic", 2);
    }
    void OnCompletedAllActions()
    {
        actionPending = false;
    }
    IEnumerator Logic()
    {
        yield return new WaitForSeconds(1);

       
        //combatManager.CompletedAllActions -= Logic;
        while (true)
        {
            yield return new WaitWhile(()=>actionPending);
            if(currentTurn.actionsLeft <= 0)
            {
                break;
            }
          
            CombatManager.Turn weakestTarget = combatManager.depths[0].TargetFirst(CombatManager.Team.player);
            int depthIndex = 0;
            
            int abilityIndex = 0;
            //int tryCount=0;
            HashSet<int> hasTried=new();


            do
            {
                if (hasTried.Count >= 3)
                {
                    break;
                    
                }

                abilityIndex = Random.Range(0, 3);
                if (hasTried.Contains(abilityIndex))
                {
                    continue;
                }
                else
                {
                    hasTried.Add(abilityIndex);
                }
               

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
                actionPending = true;
                combatManager.CompletedAllActions += OnCompletedAllActions;
            }else if (!currentTurn.fish.Type.HomeDepth.HasFlag(currentTurn.currentDepth.depth))
            {
                actionPending = true;
                combatManager.CompletedAllActions += OnCompletedAllActions;
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
                break;
            }



        }
        yield return new WaitForSeconds(1);
        EndTurn();



    }



    void EndTurn()
    {
        currentTurn.EndTurn();
    }
}

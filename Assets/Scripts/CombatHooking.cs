using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatHooking : MonoBehaviour
{
    [SerializeField]
    GameObject hook;
    [SerializeField]
    Transform hookPivot;
    LineRenderer lineRenderer;
    Rigidbody rb;
    HingeJoint joint;
    [SerializeField]
    float hookSpeed;
    // Start is called before the first frame update
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        //joint = GetComponentInChildren<HingeJoint>();
        rb = GetComponentInChildren<Rigidbody>();
        
    }

    public void HookingLocation(FishObject target,bool shouldCatch,Action Completed)
    {
        Debug.Log("should lower hook");
        StartCoroutine(LowerHook(target, shouldCatch, Completed));
    }

    IEnumerator LowerHook(FishObject target, bool shouldCatch, Action Completed)
    {
        var pos = target.hookLocation.position;
        
        while(Vector3.Distance(hook.transform.position,pos)>0)
        {
            Debug.Log(Vector3.Distance(hook.transform.position, pos));
            hook.transform.position = Vector3.MoveTowards(hook.transform.position, pos, hookSpeed*Time.deltaTime);
            //hook.transform.Translate(Vector3.down * hookSpeed * Time.fixedDeltaTime);
            lineRenderer.SetPosition(1, hook.transform.localPosition+ hookPivot.localPosition);
            yield return null;
        }

        if (shouldCatch)
        {
            target.AddComponent<Rigidbody>();
            joint = hook.AddComponent<HingeJoint>();
            target.StopMoving();
            target.GetComponentInChildren<Animator>().enabled = false;
            //joint.useLimits = true;
            //var limits = joint.limits;
            //limits.max = -45;
            //limits.min = -135;
            //joint.limits = limits;
            joint.axis = new Vector3(0, 0, 1);
            joint.connectedBody = target.GetComponent<Rigidbody>();
            joint.connectedAnchor = target.hookLocation.position;
            yield return new WaitForSeconds(1);

           
           
        }

        while (Vector3.Distance(this.transform.position, hook.transform.position) > 0)
        {
            hook.transform.position = Vector3.MoveTowards(hook.transform.position, this.transform.position, hookSpeed * 2 * Time.deltaTime);
            lineRenderer.SetPosition(1, hook.transform.localPosition);
            yield return null;
        }
        
        Completed?.Invoke();
        Destroy(this.gameObject);
    }
}

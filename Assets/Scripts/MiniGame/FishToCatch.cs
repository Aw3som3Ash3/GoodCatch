using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class FishToCatch : MonoBehaviour
{

    GameObject model;

    Transform hook;
    [SerializeField]
    float speed;
    //Vector3 destination;
    FishMonsterType fishMonster;
    //float radius;
    enum FishBehaviour
    {
        none,
        idle,
        curious,
        goingToBite,
        biting,
        reeled
    }
    FishBehaviour behaviour = FishBehaviour.none;
    float timer;
    Vector3 destination;

    public void SetIdle(Vector3 center)
    {
        
        behaviour = FishBehaviour.idle;
        
       
    }

  

    public void StartCatching(Transform hook)
    {
        //this.fishMonster = fishMonster;
        //model = Instantiate(fishMonster.Model, this.transform);
        //model.transform.localPosition = new Vector3(0, -2, 0);
        this.hook = hook;
        behaviour = FishBehaviour.curious;
        ChangeDestination();
        StartCoroutine(StateChanger());
        Debug.Log("fish spawned");
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (behaviour == FishBehaviour.idle)
        {
            
            float radius= Vector3.Distance(this.transform.parent.position , this.transform.position);
            float dist = speed*10 * Time.deltaTime;
            float rot = dist / radius;

            transform.RotateAround(this.transform.parent.position,Vector3.up, rot);
            transform.Translate(Mathf.Sin(Time.time) *0.05f* (this.transform.parent.position- this.transform.position).normalized * Time.deltaTime);
        }

        if (behaviour == FishBehaviour.curious)
        {


            MoveToDestination(destination);

            if (this.transform.localPosition == destination)
            {
                ChangeDestination();
            }
        }
        else if (behaviour == FishBehaviour.goingToBite)
        {

            Vector3 dest = transform.parent.InverseTransformPoint(hook.transform.position);
            dest.y = -2;
            MoveToDestination(dest);
            if (this.transform.localPosition == dest)
            {
                behaviour = FishBehaviour.biting;
                StartCoroutine(AnimatedHook());
                Invoke("ResetBehaviour", 0.75f);
            }
        }

    }
    public bool CatchFish()
    {
        if (behaviour == FishBehaviour.biting)
        {
            CancelInvoke();
            print("caught " + this);

            return true;
        }
        else
        {
            behaviour = FishBehaviour.idle;
            this.transform.position = this.transform.parent.position+Vector3.right*this.transform.GetComponentInParent<FishZone>().Radius;
            this.transform.rotation= this.transform.parent.rotation;
            return false;
        }
    }
    IEnumerator AnimatedHook()
    {
        Debug.Log("bitting");
        float startY = hook.position.y;
        hook.GetComponent<SimpleWaterPhysics>().enabled = false;
        while (hook.position.y > (startY - 0.5f))
        {
            hook.Translate(Vector3.down * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        while (hook.position.y < (startY))
        {
            hook.Translate(Vector3.up * 1.5f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        hook.GetComponent<SimpleWaterPhysics>().enabled = true;
        //ResetBehaviour();
    }
    void ResetBehaviour()
    {

        behaviour = FishBehaviour.curious;
        StartCoroutine(StateChanger());
    }

    void MoveToDestination(Vector3 destination)
    {
        this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, destination, speed * Time.deltaTime);
        Quaternion targetRot = Quaternion.LookRotation((destination - this.transform.localPosition).normalized);
        this.transform.localRotation = Quaternion.RotateTowards(this.transform.localRotation, targetRot, 360 * Time.deltaTime);
    }
    void ChangeDestination()
    {
        destination = new Vector3(UnityEngine.Random.Range(-5, 5), -2, UnityEngine.Random.Range(-5, 5));
    }
    IEnumerator StateChanger()
    {
        int numOfTries = 0;
        while (behaviour == FishBehaviour.curious)
        {


            if (behaviour == FishBehaviour.curious)
            {
                yield return new WaitForSeconds(3);

                if (UnityEngine.Random.Range(0, 1f) > .95f - (.1 * numOfTries))
                {
                    behaviour = FishBehaviour.goingToBite;
                    break;
                }
                else
                {
                    numOfTries++;
                }

            }


        }

    }
}

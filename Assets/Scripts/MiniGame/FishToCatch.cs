using System.Collections;
using UnityEngine;

public class FishToCatch : MonoBehaviour
{

    GameObject model;

    Transform hook;
    [SerializeField]
    float speed;
    //Vector3 destination;
    FishMonsterType fishMonster;
    enum FishBehaviour
    {
        none,
        curious,
        goingToBite,
        biting,
        reeled
    }
    FishBehaviour behaviour = FishBehaviour.none;
    float timer;
    Vector3 destination;
    public void SetFish(FishMonsterType fishMonster, Transform hook)
    {
        this.fishMonster = fishMonster;
        model = Instantiate(fishMonster.Model, this.transform);
        model.transform.localPosition = new Vector3(0, -2, 0);
        this.hook = hook;
        behaviour = FishBehaviour.curious;
        ChangDestination();
        StartCoroutine(StateChanger());
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (behaviour == FishBehaviour.curious)
        {


            MoveToDestination(destination);

            if (this.transform.localPosition == destination)
            {
                ChangDestination();
            }
        }
        else if (behaviour == FishBehaviour.goingToBite)
        {

            MoveToDestination(transform.parent.InverseTransformPoint(hook.transform.position));
            if (this.transform.localPosition == transform.parent.InverseTransformPoint(hook.transform.position))
            {
                behaviour = FishBehaviour.biting;
                StartCoroutine(AnimatedHook());
                Invoke("ResetBehaviour", 0.5f);
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
            return false;
        }
    }
    IEnumerator AnimatedHook()
    {
        float startY = hook.position.y;
        hook.GetComponent<SimpleWaterPhysics>().enabled = false;
        while (hook.position.y > (startY - 0.5f))
        {
            hook.Translate(Vector3.down * 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        while (hook.position.y < (startY))
        {
            hook.Translate(Vector3.up * 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        hook.GetComponent<SimpleWaterPhysics>().enabled = true;
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
    void ChangDestination()
    {
        destination = new Vector3(Random.Range(-5, 5), -2, Random.Range(-5, 5));
    }
    IEnumerator StateChanger()
    {
        int numOfTries = 0;
        while (behaviour == FishBehaviour.curious)
        {


            if (behaviour == FishBehaviour.curious)
            {
                yield return new WaitForSeconds(3);

                if (Random.Range(0, 1f) > .95f - (.1 * numOfTries))
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

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingRod : MonoBehaviour
{
    [SerializeField]
    Transform reel, lineStart;
    [SerializeField]
    GameObject floaterPrefab;
    Floater floater;
    LineRenderer fishingLine;
    [SerializeField]
    float castForce;

    [SerializeField]
    FishingMiniGame gamePrefab;
    private void Awake()
    {
        fishingLine = GetComponent<LineRenderer>();
        fishingLine.positionCount = 2;
        fishingLine.SetPosition(0, reel.localPosition);
        fishingLine.SetPosition(1, lineStart.localPosition);

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (floater != null)
        {
            fishingLine.SetPosition(2, this.transform.InverseTransformPoint(floater.transform.position));
        }
        else
        {
            fishingLine.positionCount = 2;
        }
    }

    public void CastLine(Vector3 lookDir)
    {
        if (floater != null)
        {
            Destroy(floater.gameObject);
        }

        floater = Instantiate(floaterPrefab, lineStart.transform.position, floaterPrefab.transform.rotation).GetComponent<Floater>();
        floater.HitWater += StartMiniGame;
        floater.GetComponent<Rigidbody>().AddForce((lookDir + Vector3.up) * castForce, ForceMode.Impulse);
        fishingLine.positionCount = 3;
    }

    void StartMiniGame()
    {
        FishingMiniGame game = Instantiate(gamePrefab, floater.transform.position, this.transform.parent.transform.rotation);
        game.Initiate(floater);
        InputManager.DisablePlayer();
        floater.HitWater -= StartMiniGame;
        FishingMiniGame.SuccesfulFishing = () => { Destroy(floater.gameObject); };
    }
}

using System;
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
    Action OnComplete;

    public AudioController audioController;

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
            Vector3 relativeFloaterPos = this.transform.InverseTransformPoint(floater.LineEndPos);
            for (int i = 2; i < fishingLine.positionCount-1; i++)
            {
                Vector3 pos = QuadBezier(lineStart.localPosition,(relativeFloaterPos - lineStart.localPosition)/2 +Vector3.down*2 + Vector3.back * 2 , relativeFloaterPos, (float)(i-2)/ (float)(fishingLine.positionCount - 2));
                fishingLine.SetPosition(i, pos);
            }
            fishingLine.SetPosition(fishingLine.positionCount - 1, relativeFloaterPos);
        }
        else
        {
            fishingLine.positionCount = 2;
        }
    }

    public void CastLine(Vector3 lookDir,float force,Action callback)
    {
        audioController.PlayClipRandom();

        if (floater != null)
        {
            Destroy(floater.gameObject);
        }
        OnComplete += callback;
        floater = Instantiate(floaterPrefab, lineStart.transform.position, floaterPrefab.transform.rotation).GetComponent<Floater>();
        floater.completed += OnComplete;
        floater.HitWater += StartMiniGame;
        floater.GetComponent<Rigidbody>().AddForce( (lookDir+Vector3.up).normalized  * castForce* force, ForceMode.Impulse);
        fishingLine.positionCount = 20;
        
    }

    void StartMiniGame()
    {
        FishingMiniGame game = Instantiate(gamePrefab, floater.transform.position, this.transform.parent.transform.rotation);
        game.Initiate(floater);
        game.OnCancel += OnComplete;
        InputManager.DisablePlayer();
        floater.HitWater -= StartMiniGame;
        FishingMiniGame.SuccesfulFishing += FishingCompleted;
    }

    void FishingCompleted()
    {
        Destroy(floater.gameObject); 
        OnComplete?.Invoke();
        FishingMiniGame.SuccesfulFishing -= FishingCompleted;
    }

    Vector3 QuadBezier(Vector3 start,Vector3 controlPoint, Vector3 end,float t)
    {

        return (1 - t) * (1 - t) * start + 2 * (1 - t) * t * controlPoint + t * t * end;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishZone : SaveableObject
{
    [SerializeField]
    SpawnTables spawnTable;
    

    [SerializeField]
    int minAmount, maxAmount;

    [Serializable]
    struct Data
    {
        [SerializeField]
        public int amount;
    }
    [SerializeField]
    Data data;
    [SerializeField]
    GameObject fishPrefab;
    List<FishToCatch> fishes=new();
    [HideInInspector]
    public override object DataToSave => data;
    [SerializeField]
    float radius;
    public float Radius { get {return radius; } }

    // Start is called before the first frame update
    void Start()
    {
        data.amount = UnityEngine.Random.Range(minAmount, maxAmount + 1);

        for (int i = 0; i < data.amount; i++)
        {
            var fish = GameObject.Instantiate(fishPrefab, this.transform).GetComponent<FishToCatch>();
            fish.transform.rotation = this.transform.rotation;
            fishes.Add(fish);
            var rot = Quaternion.AngleAxis(((float)i / data.amount * 360), Vector3.up);
            var dir = (rot * Vector3.right);
            fish.transform.localPosition= dir * 1 * (radius + UnityEngine.Random.Range(-0.5f,0.5f));
            fish.transform.localRotation = rot;
            
            fish.SetIdle(this.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public FishToCatch GetFishObect()
    {
        return fishes[UnityEngine.Random.Range(0, fishes.Count)];
    }
    public FishMonster GetRandomFish()
    {
        if (data.amount < 0)
        {
            return null;
        }
        data.amount--;
        return spawnTable.GetRandomFish();
    }
    public override void Load(string json)
    {
        //JsonUtility.FromJsonOverwrite(json, this);
        var data = JsonUtility.FromJson<Data>(json);
        this.data = data;

    }

    private void OnDrawGizmos()
    {
        var col =this.GetComponent<Collider>();
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        
        
        Gizmos.DrawCube(col.bounds.center,new Vector3(col.bounds.size.x, col.bounds.size.y, col.bounds.size.z));
    }
}

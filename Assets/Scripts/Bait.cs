using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Bait", menuName = "Fish Monster/Bait", order = 3)]
public class Bait : ScriptableObject
{
   enum Quality
    {
        poor=5,
        normal=15,
        good=25,
        surperb=45,
        legendary=65
    }

    [SerializeField]
    Quality quality;
    //public class BaitInstance
    //{
    //    public Bait bait { get; private set; }
        

    //}
}



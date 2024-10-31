using UnityEngine;
[CreateAssetMenu(fileName = "Bait", menuName = "Items/Bait", order = 3)]
public class Bait : Item
{
    enum Quality
    {
        poor = 5,
        normal = 15,
        good = 25,
        surperb = 45,
        legendary = 65
    }

    [SerializeField]
    Quality quality;
    //public class BaitInstance
    //{
    //    public Bait bait { get; private set; }


    //}
}




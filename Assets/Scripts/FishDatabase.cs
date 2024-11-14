using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[FilePath("Assets/Database/FishDatabase", FilePathAttribute.Location.ProjectFolder)]
#endif
public class FishDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    public List<FishMonsterType> fishMonsters;
    //public IReadOnlyList<FishMonsterType> FishMonsters { get { return fishMonsters; } }
    public Dictionary<int, FishMonsterType> GetFish = new Dictionary<int, FishMonsterType>();
    public Dictionary<FishMonsterType, int> GetId = new Dictionary<FishMonsterType, int>();
    public void OnAfterDeserialize()
    {
        GetId = new Dictionary<FishMonsterType, int>();

        fishMonsters = fishMonsters.GroupBy(x => x).Select(y => y.First()).ToList();
        for (int i = 0; i < fishMonsters.Count; i++)
        {
            GetId.Add(fishMonsters[i], i);
            fishMonsters[i].fishId = i;
            GetFish.Add(i, fishMonsters[i]);
        }

    }
    public void SetList(List<FishMonsterType> fishMonsters)
    {

    }
    public void OnBeforeSerialize()
    {
        GetFish = new Dictionary<int, FishMonsterType>();
    }
    public FishMonsterType GetRandom()
    {
        return fishMonsters[1];
        //return fishMonsters[Random.Range(0, fishMonsters.Count)];
    }
    //public void SaveDatabase()
    //{

    //    Debug.Log(GetFilePath());
    //    Save(false);
    //}
}
//#if UNITY_EDITOR
//public class WizardFishDataBase : ScriptableWizard
//{
//    [SerializeField]
//    List<FishMonsterType> fishMonsters;

//    [MenuItem("FishDatabase/Edit FishDatabase")]
//    static void CreateWizard()
//    {
//        //
//        ScriptableWizard.DisplayWizard<WizardFishDataBase>("Fish Database","Save");
//        //If you don't want to use the secondary button simply leave it out:
//        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
//    }

//    void OnWizardCreate()
//    {
//        FishDatabase.instance.SaveDatabase();
//    }

//    void OnWizardUpdate()
//    {
//        fishMonsters = FishDatabase.instance.fishMonsters;
//        //helpString = "Please set the color of the light!";
//    }

//    // When the user presses the "Apply" button OnWizardOtherButton is called.
//    void OnWizardOtherButton()
//    {
//        //if (Selection.activeTransform != null)
//        //{
//        //    Light lt = Selection.activeTransform.GetComponent<Light>();

//        //    if (lt != null)
//        //    {
//        //        lt.color = Color.red;
//        //    }
//        //}
//    }
//}
//#endif
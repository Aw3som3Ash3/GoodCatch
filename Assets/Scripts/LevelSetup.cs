using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
[CreateAssetMenu(fileName ="SceneLayout",menuName ="SceneLayout")]
public class LevelSetup : ScriptableObject
{
    [SerializeField]
    public SceneSetup[] sceneSetup;
    
}

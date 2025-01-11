using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;

[CreateAssetMenu(fileName ="SceneLayout",menuName ="SceneLayout")]
public class LevelSetup : ScriptableObject
{
    [SerializeField]
    public SceneSetup[] sceneSetup;
    
}
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorSceneLoader : MonoBehaviour
{
    [SerializeField]
    LevelSetup sceneSetup;
    Scene scene;


    private void SceneClosed(Scene scene, bool removingScene)
    {
        if (this.scene == scene)
        {
            EditorSceneManager.sceneOpened -= EditorLoadScene;
            EditorSceneManager.sceneClosing -= SceneClosed;
            sceneSetup.sceneSetup = EditorSceneManager.GetSceneManagerSetup();

        }
    }

    private void EditorLoadScene(Scene scene, OpenSceneMode mode)
    {
        scene = EditorSceneManager.GetActiveScene();
        if (sceneSetup.sceneSetup.Length > 0 && scene.path == sceneSetup.sceneSetup[0].path)
        {
            EditorSceneManager.RestoreSceneManagerSetup(sceneSetup.sceneSetup);
        }


    }
    private void OnValidate()
    {
        scene=EditorSceneManager.GetActiveScene();
        EditorSceneManager.sceneOpened += EditorLoadScene;
        EditorSceneManager.sceneClosing += SceneClosed;
    }

    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
